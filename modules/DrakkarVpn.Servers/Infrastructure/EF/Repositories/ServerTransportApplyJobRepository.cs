using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Common.Guards;
using DrakkarVpn.Servers.Domain.Enums.TransportProfile;
using DrakkarVpn.Servers.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace DrakkarVpn.Servers.Infrastructure.EF.Repositories;

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
        long targetTransportVersion,
        int maxAttempt,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (maxAttempt <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt));

        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

        await SupersedeOlderInFlightJobsAsync(
            serverId,
            targetTransportVersion,
            utcNow,
            ct);

        var existingJobId = await FindInFlightJobIdAsync(serverId, targetTransportVersion, ct);
        if (existingJobId.HasValue)
            return existingJobId.Value;

        var job = ServerTransportApplyJob.CreateNew(
            serverId,
            activationId,
            targetTransportVersion,
            maxAttempt,
            utcNow);

        _db.Set<ServerTransportApplyJob>().Add(job);

        return job.JobId;
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

        leaseOwner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);
        var leaseUntilUtc = utcNow.Add(lease);

        var pending = (short)ServerTransportApplyJobStatus.Pending;
        var processing = (short)ServerTransportApplyJobStatus.Processing;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var acquired = await _db.Set<ServerTransportApplyJob>()
            .FromSqlInterpolated($"""
                                  WITH picked AS (
                                      SELECT j.job_id
                                      FROM servers.server_transport_apply_jobs j
                                      WHERE
                                          (
                                              j.state = {pending}
                                              AND j.next_attempt_utc <= {utcNow}
                                          )
                                          OR
                                          (
                                              j.state = {processing}
                                              AND j.lease_until_utc IS NOT NULL
                                              AND j.lease_until_utc <= {utcNow}
                                          )
                                      ORDER BY j.created_at_utc
                                      LIMIT {take}
                                      FOR UPDATE SKIP LOCKED
                                  ),
                                  leased AS (
                                      UPDATE servers.server_transport_apply_jobs u
                                      SET
                                          state = {processing},
                                          lease_owner = {leaseOwner},
                                          lease_until_utc = {leaseUntilUtc},
                                          updated_at_utc = {utcNow}
                                      FROM picked p
                                      WHERE u.job_id = p.job_id
                                      RETURNING u.*
                                  )
                                  SELECT *
                                  FROM leased
                                  ORDER BY created_at_utc;
                                  """)
            .AsNoTracking()
            .ToListAsync(ct);

        await tx.CommitAsync(ct);

        return acquired;
    }

    public Task<int> MarkCompletedAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (jobIds.Count == 0)
            return Task.FromResult(0);

        leaseOwner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

        return _db.Set<ServerTransportApplyJob>()
            .Where(x => jobIds.Contains(x.JobId)
                        && x.LeaseOwner == leaseOwner
                        && x.State == ServerTransportApplyJobStatus.Processing)
            .ExecuteUpdateAsync(set => set
                .SetProperty(x => x.State, ServerTransportApplyJobStatus.Completed)
                .SetProperty(x => x.CompletedAtUtc, utcNow)
                .SetProperty(x => x.LastErrorCode, (string?)null)
                .SetProperty(x => x.LastErrorMessage, (string?)null)
                .SetProperty(x => x.LeaseOwner, (string?)null)
                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                .SetProperty(x => x.UpdatedAtUtc, utcNow), ct);
    }

    public async Task<int> MarkFailedBatchAsync(
        IReadOnlyList<ServerTransportApplyJobMarkFailedBatchRow> rows,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (rows.Count == 0)
            return 0;

        leaseOwner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

        var failedState = (int)ServerTransportApplyJobStatus.Failed;
        var processingState = (int)ServerTransportApplyJobStatus.Processing;

        var n = rows.Count;
        var jobIds = new Guid[n];
        var codes = new string[n];
        var messages = new string?[n];
        for (var i = 0; i < n; i++)
        {
            jobIds[i] = rows[i].JobId;
            codes[i] = rows[i].ErrorCode;
            messages[i] = rows[i].ErrorMessage;
        }

        const string sql = """
            UPDATE servers.server_transport_apply_jobs j
            SET
                state = @failed_state,
                last_error_code = u.error_code,
                last_error_message = u.error_message,
                lease_owner = NULL,
                lease_until_utc = NULL,
                updated_at_utc = @utc_now
            FROM unnest(@job_ids::uuid[], @error_codes::text[], @error_messages::text[])
                AS u(job_id, error_code, error_message)
            WHERE j.job_id = u.job_id
              AND j.lease_owner = @lease_owner
              AND j.state = @processing_state
            """;

        return await _db.Database.ExecuteSqlRawAsync(
            sql,
            ct,
            new NpgsqlParameter("failed_state", failedState),
            new NpgsqlParameter("processing_state", processingState),
            new NpgsqlParameter("utc_now", utcNow) { NpgsqlDbType = NpgsqlDbType.TimestampTz },
            new NpgsqlParameter("lease_owner", leaseOwner),
            new NpgsqlParameter("job_ids", jobIds) { DataTypeName = "uuid[]" },
            new NpgsqlParameter("error_codes", codes) { DataTypeName = "text[]" },
            new NpgsqlParameter("error_messages", messages) { DataTypeName = "text[]" });
    }

    public async Task<int> RescheduleBatchAsync(
        IReadOnlyList<ServerTransportApplyJobRescheduleBatchRow> rows,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (rows.Count == 0)
            return 0;

        leaseOwner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

        var pendingState = (int)ServerTransportApplyJobStatus.Pending;
        var processingState = (int)ServerTransportApplyJobStatus.Processing;

        var n = rows.Count;
        var jobIds = new Guid[n];
        var newAttempts = new int[n];
        var nextAts = new DateTime[n];
        var codes = new string[n];
        var messages = new string?[n];
        for (var i = 0; i < n; i++)
        {
            jobIds[i] = rows[i].JobId;
            newAttempts[i] = rows[i].NewAttempt;
            nextAts[i] = UtcDateTimeGuard.RequireUtc(rows[i].NextAttemptAtUtc);
            codes[i] = rows[i].ErrorCode;
            messages[i] = rows[i].ErrorMessage;
        }

        const string sql = """
            UPDATE servers.server_transport_apply_jobs j
            SET
                state = @pending_state,
                attempt = u.new_attempt,
                next_attempt_utc = u.next_attempt_at,
                last_error_code = u.error_code,
                last_error_message = u.error_message,
                lease_owner = NULL,
                lease_until_utc = NULL,
                updated_at_utc = @utc_now
            FROM unnest(
                    @job_ids::uuid[],
                    @new_attempts::int[],
                    @next_attempt_ats::timestamptz[],
                    @error_codes::text[],
                    @error_messages::text[])
                AS u(job_id, new_attempt, next_attempt_at, error_code, error_message)
            WHERE j.job_id = u.job_id
              AND j.lease_owner = @lease_owner
              AND j.state = @processing_state
            """;

        return await _db.Database.ExecuteSqlRawAsync(
            sql,
            ct,
            new NpgsqlParameter("pending_state", pendingState),
            new NpgsqlParameter("processing_state", processingState),
            new NpgsqlParameter("utc_now", utcNow) { NpgsqlDbType = NpgsqlDbType.TimestampTz },
            new NpgsqlParameter("lease_owner", leaseOwner),
            new NpgsqlParameter("job_ids", jobIds) { DataTypeName = "uuid[]" },
            new NpgsqlParameter("new_attempts", newAttempts) { NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Integer },
            new NpgsqlParameter("next_attempt_ats", nextAts) { NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.TimestampTz },
            new NpgsqlParameter("error_codes", codes) { DataTypeName = "text[]" },
            new NpgsqlParameter("error_messages", messages) { DataTypeName = "text[]" });
    }

    public Task<int> MarkObsoleteAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (jobIds.Count == 0)
            return Task.FromResult(0);

        leaseOwner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

        return _db.Set<ServerTransportApplyJob>()
            .Where(x => jobIds.Contains(x.JobId)
                        && x.LeaseOwner == leaseOwner
                        && x.State == ServerTransportApplyJobStatus.Processing)
            .ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.State, ServerTransportApplyJobStatus.Obsolete)
                    .SetProperty(x => x.LeaseOwner, (string?)null)
                    .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                    .SetProperty(x => x.UpdatedAtUtc, utcNow),
                ct);
    }

    private Task<int> SupersedeOlderInFlightJobsAsync(
        Guid serverId,
        long targetTransportVersion,
        DateTime utcNow,
        CancellationToken ct)
    {
        var pending = ServerTransportApplyJobStatus.Pending;
        var processing = ServerTransportApplyJobStatus.Processing;

        return _db.Set<ServerTransportApplyJob>()
            .Where(x => x.ServerId == serverId
                        && x.TargetTransportVersion < targetTransportVersion
                        && (x.State == pending || x.State == processing))
            .ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.State, ServerTransportApplyJobStatus.Obsolete)
                    .SetProperty(x => x.LeaseOwner, (string?)null)
                    .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null)
                    .SetProperty(x => x.UpdatedAtUtc, utcNow),
                ct);
    }

    private Task<Guid?> FindInFlightJobIdAsync(
        Guid serverId,
        long targetTransportVersion,
        CancellationToken ct)
        => _db.Set<ServerTransportApplyJob>()
            .AsNoTracking()
            .Where(x => x.ServerId == serverId
                        && x.TargetTransportVersion == targetTransportVersion
                        && (x.State == ServerTransportApplyJobStatus.Pending
                            || x.State == ServerTransportApplyJobStatus.Processing))
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => (Guid?)x.JobId)
            .FirstOrDefaultAsync(ct);
}
