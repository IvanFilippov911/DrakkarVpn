using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Infrastructure.EF.Entities;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class ServerTransportApplyJobService : IServerTransportApplyJobService
{
    private readonly IServerTransportApplyJobRepository _repo;

    private const int DefaultMaxAttempt = 10;
    private const int MaxTake = 500;
    private const int MaxErrorCodeLen = 128;
    private const int MaxErrorMessageLen = 2048;
    private static readonly TimeSpan MinLease = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaxLease = TimeSpan.FromMinutes(5);

    public ServerTransportApplyJobService(IServerTransportApplyJobRepository repo)
    {
        _repo = repo;
    }

    public Task<Guid> EnqueueAsync(
        Guid serverId,
        Guid activationId,
        long targetTransportVersion,
        CancellationToken ct)
    {
        if (serverId == Guid.Empty) throw new ArgumentException("serverId is required", nameof(serverId));
        if (activationId == Guid.Empty) throw new ArgumentException("activationId is required", nameof(activationId));
        if (targetTransportVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetTransportVersion));

        return _repo.CreateOrGetAsync(
            serverId,
            activationId,
            targetTransportVersion,
            DefaultMaxAttempt,
            DateTime.UtcNow,
            ct);
    }

    public Task<Guid?> GetActiveJobIdByServerIdAsync(Guid serverId, CancellationToken ct)
    {
        if (serverId == Guid.Empty) throw new ArgumentException("serverId is required", nameof(serverId));
        return _repo.GetActiveJobIdByServerIdAsync(serverId, ct);
    }

    public async Task<IReadOnlyList<ServerTransportApplyJobDto>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime utcNow,
        string leaseOwner,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(leaseOwner))
            throw new ArgumentException("leaseOwner is required", nameof(leaseOwner));
        if (take <= 0)
            return [];

        if (take > MaxTake) take = MaxTake;
        lease = Clamp(lease, MinLease, MaxLease);
        utcNow = EnsureUtc(utcNow);

        var rows = await _repo.AcquireBatchAsync(
            take: take,
            lease: lease,
            utcNow: utcNow,
            leaseOwner: leaseOwner.Trim(),
            ct: ct);

        return rows.Select(Map).ToList();
    }

    public async Task<ServerTransportApplyJobDto?> GetAsync(Guid jobId, CancellationToken ct)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("jobId is required", nameof(jobId));
        var row = await _repo.GetByIdAsync(jobId, ct);
        return row is null ? null : Map(row);
    }

    public Task<int> MarkCompletedAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (jobIds.Count == 0)
            return Task.FromResult(0);
        if (string.IsNullOrWhiteSpace(leaseOwner))
            throw new ArgumentException("leaseOwner is required", nameof(leaseOwner));

        return _repo.MarkCompletedAsync(
            jobIds,
            leaseOwner.Trim(),
            EnsureUtc(utcNow),
            ct);
    }

    public Task<int> FailPermanentAsync(
        IReadOnlyCollection<ServerTransportApplyJobFailure> failures,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (failures.Count == 0)
            return Task.FromResult(0);
        if (string.IsNullOrWhiteSpace(leaseOwner))
            throw new ArgumentException("leaseOwner is required", nameof(leaseOwner));

        utcNow = EnsureUtc(utcNow);
        var owner = leaseOwner.Trim();

        var rows = new List<ServerTransportApplyJobMarkFailedBatchRow>(failures.Count);
        foreach (var failure in failures)
        {
            if (failure.Job.JobId == Guid.Empty) continue;

            rows.Add(new ServerTransportApplyJobMarkFailedBatchRow(
                failure.Job.JobId,
                NormalizeErrorCode(failure.ErrorCode),
                NormalizeErrorMessage(failure.ErrorMessage)));
        }

        if (rows.Count == 0)
            return Task.FromResult(0);

        return _repo.MarkFailedBatchAsync(rows, owner, utcNow, ct);
    }

    public async Task<int> FailOrRescheduleAsync(
        IReadOnlyCollection<ServerTransportApplyJobFailure> failures,
        string leaseOwner,
        DateTime utcNow,
        Func<int, TimeSpan> backoff,
        CancellationToken ct)
    {
        if (failures.Count == 0)
            return 0;
        if (string.IsNullOrWhiteSpace(leaseOwner))
            throw new ArgumentException("leaseOwner is required", nameof(leaseOwner));
        if (backoff is null)
            throw new ArgumentNullException(nameof(backoff));

        utcNow = EnsureUtc(utcNow);
        var owner = leaseOwner.Trim();

        var toFail = new List<ServerTransportApplyJobMarkFailedBatchRow>();
        var toReschedule = new List<ServerTransportApplyJobRescheduleBatchRow>();

        foreach (var failure in failures)
        {
            var job = failure.Job;
            if (job.JobId == Guid.Empty) continue;
            if (job.MaxAttempt <= 0) continue;

            var code = NormalizeErrorCode(failure.ErrorCode);
            var message = NormalizeErrorMessage(failure.ErrorMessage);
            var nextAttempt = job.Attempt + 1;

            if (nextAttempt >= job.MaxAttempt)
                toFail.Add(new ServerTransportApplyJobMarkFailedBatchRow(job.JobId, code, message));
            else
            {
                var delay = backoff(nextAttempt);
                if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

                toReschedule.Add(new ServerTransportApplyJobRescheduleBatchRow(
                    job.JobId,
                    nextAttempt,
                    utcNow.Add(delay),
                    code,
                    message));
            }
        }

        var total = 0;
        if (toFail.Count > 0)
            total += await _repo.MarkFailedBatchAsync(toFail, owner, utcNow, ct);
        if (toReschedule.Count > 0)
            total += await _repo.RescheduleBatchAsync(toReschedule, owner, utcNow, ct);

        return total;
    }

    public Task<int> MarkObsoleteAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (jobIds.Count == 0) return Task.FromResult(0);
        if (string.IsNullOrWhiteSpace(leaseOwner))
            throw new ArgumentException("LeaseOwner is required");
        
        return _repo.MarkObsoleteAsync(
            jobIds,
            leaseOwner.Trim(),
            EnsureUtc(utcNow),
            ct);
    }

    private static ServerTransportApplyJobDto Map(ServerTransportApplyJob row)
        => new(
            row.JobId,
            row.ServerId,
            row.ActivationId,
            row.TargetTransportVersion,
            row.State,
            row.LeaseUntilUtc,
            row.LeaseOwner,
            row.Attempt,
            row.MaxAttempt,
            row.NextAttemptUtc,
            row.LastErrorCode,
            row.LastErrorMessage,
            row.CreatedAtUtc,
            row.UpdatedAtUtc,
            row.CompletedAtUtc);

    private static string NormalizeErrorCode(string errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
            errorCode = "Unknown";
        errorCode = errorCode.Trim();
        return errorCode.Length > MaxErrorCodeLen ? errorCode[..MaxErrorCodeLen] : errorCode;
    }

    private static string? NormalizeErrorMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;
        message = message.Trim();
        return message.Length > MaxErrorMessageLen ? message[..MaxErrorMessageLen] : message;
    }

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    private static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
        => value < min ? min : (value > max ? max : value);
}
