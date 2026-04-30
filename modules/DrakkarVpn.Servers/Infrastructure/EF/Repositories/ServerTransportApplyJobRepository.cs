using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Domain.Enums.TransportProfile;
using DrakkarVpn.Servers.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerTransportApplyJobRepository : IServerTransportApplyJobRepository
{
    private readonly ServerDbContext _db;

    public ServerTransportApplyJobRepository(ServerDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateOrGetAsync(
        Guid serverId,
        Guid activationId,
        int maxAttempt,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (serverId == Guid.Empty) throw new ArgumentException("serverId is required", nameof(serverId));
        if (activationId == Guid.Empty) throw new ArgumentException("activationId is required", nameof(activationId));
        if (maxAttempt <= 0) throw new ArgumentException("maxAttempt must be > 0", nameof(maxAttempt));

        var existing = await FindActiveJobIdByServerIdAsync(serverId, ct);
        if (existing != Guid.Empty)
            return existing;

        var job = ServerTransportApplyJob.CreateNew(serverId, activationId, maxAttempt, EnsureUtc(utcNow));
        _db.Set<ServerTransportApplyJob>().Add(job);

        try
        {
            await _db.SaveChangesAsync(ct);
            return job.JobId;
        }
        catch (DbUpdateException)
        {
            _db.Entry(job).State = EntityState.Detached;
            var existingId = await FindActiveJobIdByServerIdAsync(serverId, ct);
            if (existingId == Guid.Empty)
                throw;
            return existingId;
        }
    }

    public async Task<Guid?> GetActiveJobIdByServerIdAsync(Guid serverId, CancellationToken ct)
    {
        if (serverId == Guid.Empty) throw new ArgumentException("serverId is required", nameof(serverId));

        var id = await FindActiveJobIdByServerIdAsync(serverId, ct);
        return id == Guid.Empty ? null : id;
    }

    public Task<ServerTransportApplyJob?> GetByIdAsync(Guid jobId, CancellationToken ct)
        => _db.Set<ServerTransportApplyJob>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobId == jobId, ct);

    public async Task<IReadOnlyList<ServerTransportApplyJob>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime utcNow,
        string leaseOwner,
        CancellationToken ct)
    {
        if (take <= 0)
            return Array.Empty<ServerTransportApplyJob>();

        utcNow = EnsureUtc(utcNow);
        var jobIds = await _db.Set<ServerTransportApplyJob>()
            .AsNoTracking()
            .Where(x => x.State == ServerTransportApplyJobStatus.Pending)
            .Where(x => x.NextAttemptUtc <= utcNow)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => x.JobId)
            .Take(take)
            .ToListAsync(ct);

        if (jobIds.Count == 0)
            return Array.Empty<ServerTransportApplyJob>();

        await _db.Set<ServerTransportApplyJob>()
            .Where(x => jobIds.Contains(x.JobId))
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, ServerTransportApplyJobStatus.Processing)
                .SetProperty(x => x.UpdatedAtUtc, utcNow), ct);

        return await _db.Set<ServerTransportApplyJob>()
            .AsNoTracking()
            .Where(x => jobIds.Contains(x.JobId))
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public Task MarkCompletedAsync(Guid jobId, DateTime utcNow, CancellationToken ct)
    {
        utcNow = EnsureUtc(utcNow);
        return _db.Set<ServerTransportApplyJob>()
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, ServerTransportApplyJobStatus.Completed)
                .SetProperty(x => x.CompletedAtUtc, utcNow)
                .SetProperty(x => x.LastErrorCode, (string?)null)
                .SetProperty(x => x.LastErrorMessage, (string?)null)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, utcNow), ct);
    }

    public Task RescheduleAsync(
        Guid jobId,
        int newAttempt,
        DateTime nextAttemptAtUtc,
        string code,
        string? message,
        DateTime utcNow,
        CancellationToken ct)
    {
        utcNow = EnsureUtc(utcNow);
        nextAttemptAtUtc = EnsureUtc(nextAttemptAtUtc);

        return _db.Set<ServerTransportApplyJob>()
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, ServerTransportApplyJobStatus.Pending)
                .SetProperty(x => x.Attempt, newAttempt)
                .SetProperty(x => x.NextAttemptUtc, nextAttemptAtUtc)
                .SetProperty(x => x.LastErrorCode, code)
                .SetProperty(x => x.LastErrorMessage, message)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, utcNow), ct);
    }

    public Task MarkFailedAsync(
        Guid jobId,
        string code,
        string? message,
        DateTime utcNow,
        CancellationToken ct)
    {
        utcNow = EnsureUtc(utcNow);

        return _db.Set<ServerTransportApplyJob>()
            .Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, ServerTransportApplyJobStatus.Failed)
                .SetProperty(x => x.LastErrorCode, code)
                .SetProperty(x => x.LastErrorMessage, message)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, utcNow), ct);
    }

    private Task<Guid> FindActiveJobIdByServerIdAsync(Guid serverId, CancellationToken ct)
        => _db.Set<ServerTransportApplyJob>()
            .AsNoTracking()
            .Where(x => x.ServerId == serverId
                        && x.State != ServerTransportApplyJobStatus.Completed
                        && x.State != ServerTransportApplyJobStatus.Failed)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => x.JobId)
            .FirstOrDefaultAsync(ct);

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}