using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerProvisionJobsRepository : IPeerProvisionJobsRepository
{
    private readonly PeerDbContext _db;

    public PeerProvisionJobsRepository(PeerDbContext db) => _db = db;

    public async Task<Guid> CreateOrGetAsync(
        PeerProvisionJobCreateDto dto,
        DateTime nowUtc,
        CancellationToken ct)
    {
        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        var existing = await _db.PeerProvisionJobs
            .AsNoTracking()
            .Where(x => x.DeviceId == dto.DeviceId
                        && x.State != PeerProvisionState.Failed
                        && x.State != PeerProvisionState.Ready)
            .Select(x => x.JobId)
            .FirstOrDefaultAsync(ct);

        if (existing != Guid.Empty)
            return existing;

        var maxAttempts = dto.MaxAttempts <= 0 ? 10 : dto.MaxAttempts;

        var job = PeerProvisionJob.CreatePrepared(
            userId: dto.UserId,
            deviceId: dto.DeviceId,
            serverId: dto.ServerId,
            agentPeerUuid: dto.AgentPeerUuid,
            configRaw: dto.ConfigRaw,
            maxAttempts: maxAttempts,
            nowUtc: nowUtc
        );

        _db.PeerProvisionJobs.Add(job);

        try
        {
            await _db.SaveChangesAsync(ct);
            return job.JobId;
        }
        catch (DbUpdateException)
        {
            var existingJobId = await _db.PeerProvisionJobs
                .AsNoTracking()
                .Where(x => x.DeviceId == dto.DeviceId
                            && x.State != PeerProvisionState.Failed
                            && x.State != PeerProvisionState.Ready)
                .OrderByDescending(x => x.CreatedAtUtc)  
                .Select(x => x.JobId)
                .FirstOrDefaultAsync(ct);

            if (existingJobId == Guid.Empty)
                throw;
            return existingJobId;
        }
    }
    
    public async Task<IReadOnlyList<PeerProvisionJob>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct)
    {
        if (take <= 0)
            return Array.Empty<PeerProvisionJob>();

        if (string.IsNullOrWhiteSpace(instanceId))
            throw new ArgumentException("instanceId is required", nameof(instanceId));

        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);
        var leaseUntil = nowUtc.Add(lease);

        var pending = (short)PeerProvisionState.Pending;
        var processing = (short)PeerProvisionState.Processing;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var leased = await _db.PeerProvisionJobs
            .FromSqlInterpolated($"""
                                  WITH picked AS (
                                      SELECT j."JobId"
                                      FROM peers.peer_provision_jobs j
                                      WHERE
                                          j."NextAttemptAtUtc" <= {nowUtc}
                                          AND (j."LeaseUntilUtc" IS NULL OR j."LeaseUntilUtc" <= {nowUtc})
                                          AND j."State" = {pending}
                                      ORDER BY j."CreatedAtUtc"
                                      LIMIT {take}
                                      FOR UPDATE SKIP LOCKED
                                  ),
                                  leased AS (
                                      UPDATE peers.peer_provision_jobs u
                                      SET
                                          "State" = {processing},
                                          "LeaseOwner" = {instanceId},
                                          "LeaseUntilUtc" = {leaseUntil},
                                          "UpdatedAtUtc" = {nowUtc}
                                      FROM picked p
                                      WHERE u."JobId" = p."JobId"
                                      RETURNING u.*
                                  )
                                  SELECT *
                                  FROM leased
                                  ORDER BY "CreatedAtUtc";
                                  """)
            .AsNoTracking()
            .ToListAsync(ct);

        await tx.CommitAsync(ct);
        return leased;
    }


    public Task<PeerProvisionJob?> GetByIdAsync(Guid jobId, CancellationToken ct)
        => _db.PeerProvisionJobs.AsNoTracking().FirstOrDefaultAsync(x => x.JobId == jobId, ct);

    public Task<Guid?> GetActiveJobIdByDeviceIdAsync(string deviceId, CancellationToken ct)
        => _db.PeerProvisionJobs
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId
                        && x.State != PeerProvisionState.Failed
                        && x.State != PeerProvisionState.Ready)
            .Select(x => (Guid?)x.JobId)
            .FirstOrDefaultAsync(ct);

    public Task MarkAgentAppliedAsync(Guid jobId, DateTime nowUtc, CancellationToken ct)
    {
        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        return _db.PeerProvisionJobs
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.AgentAppliedAtUtc, nowUtc)
                .SetProperty(x => x.LastErrorCode, (string?)null)
                .SetProperty(x => x.LastErrorMessage, (string?)null)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, nowUtc)
                .SetProperty(x => x.NextAttemptAtUtc, nowUtc), ct);
    }

    public Task MarkReadyAsync(Guid jobId, Guid peerId, DateTime nowUtc, CancellationToken ct)
    {
        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        return _db.PeerProvisionJobs
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, PeerProvisionState.Ready)
                .SetProperty(x => x.PeerId, peerId)
                .SetProperty(x => x.LastErrorCode, (string?)null)
                .SetProperty(x => x.LastErrorMessage, (string?)null)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, nowUtc), ct);
    }

    public Task RescheduleAsync(Guid jobId, int newAttempt, DateTime nextAttemptAtUtc, string code, string? message, DateTime nowUtc, CancellationToken ct)
    {
        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);
        nextAttemptAtUtc = DateTime.SpecifyKind(nextAttemptAtUtc, DateTimeKind.Utc);

        return _db.PeerProvisionJobs
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, PeerProvisionState.Pending)
                .SetProperty(x => x.Attempt, newAttempt)
                .SetProperty(x => x.NextAttemptAtUtc, nextAttemptAtUtc)
                .SetProperty(x => x.LastErrorCode, code)
                .SetProperty(x => x.LastErrorMessage, message)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, nowUtc), ct);
    }

    public Task MarkFailedAsync(Guid jobId, string code, string? message, DateTime nowUtc, CancellationToken ct)
    {
        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        return _db.PeerProvisionJobs
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, PeerProvisionState.Failed)
                .SetProperty(x => x.LastErrorCode, code)
                .SetProperty(x => x.LastErrorMessage, message)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, nowUtc), ct);
    }
}