using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Common.Guards;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.Mappers;
using DrakkarVpn.Servers.Application.Options;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;

public sealed class ServerTransportApplyJobService : IServerTransportApplyJobService
{
    private readonly IServerTransportApplyJobRepository _repo;
    private readonly ServerTransportApplyJobOptions _opt;

    public ServerTransportApplyJobService(
        IServerTransportApplyJobRepository repo,
        IOptions<ServerTransportApplyJobOptions> options)
    {
        _repo = repo;
        _opt = options.Value;
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
            _opt.DefaultMaxAttempts,
            DateTime.UtcNow,
            ct);
    }
    
    public async Task<IReadOnlyList<ServerTransportApplyJobDto>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime utcNow,
        string leaseOwner,
        CancellationToken ct)
    {
        var owner = LeaseOwnerGuard.Require(leaseOwner);
        if (take <= 0)
            return [];

        if (take > _opt.MaxAcquireBatchSize) take = _opt.MaxAcquireBatchSize;
        lease = Clamp(lease, _opt.MinLeaseDuration, _opt.MaxLeaseDuration);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

        var rows = await _repo.AcquireBatchAsync(
            take: take,
            lease: lease,
            utcNow: utcNow,
            leaseOwner: owner,
            ct: ct);

        return rows.Select(x => x.ToDto()).ToList();
    }

    public async Task<ServerTransportApplyJobDto?> GetAsync(Guid jobId, CancellationToken ct)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("jobId is required", nameof(jobId));
        var row = await _repo.GetByIdAsync(jobId, ct);
        return row is null ? null : row.ToDto();
    }

    public Task<int> MarkCompletedAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(jobIds);
        if (jobIds.Count == 0)
            return Task.FromResult(0);

        return _repo.MarkCompletedAsync(
            jobIds,
            LeaseOwnerGuard.Require(leaseOwner),
            UtcDateTimeGuard.RequireUtc(utcNow),
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

        var owner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

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
        ArgumentNullException.ThrowIfNull(backoff);

        var owner = LeaseOwnerGuard.Require(leaseOwner);
        utcNow = UtcDateTimeGuard.RequireUtc(utcNow);

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
        ArgumentNullException.ThrowIfNull(jobIds);
        if (jobIds.Count == 0) return Task.FromResult(0);

        return _repo.MarkObsoleteAsync(
            jobIds,
            LeaseOwnerGuard.Require(leaseOwner),
            UtcDateTimeGuard.RequireUtc(utcNow),
            ct);
    }

    private string NormalizeErrorCode(string errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
            errorCode = "Unknown";
        errorCode = errorCode.Trim();
        return errorCode.Length > _opt.MaxErrorCodeLength
            ? errorCode[.._opt.MaxErrorCodeLength]
            : errorCode;
    }

    private string? NormalizeErrorMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;
        message = message.Trim();
        return message.Length > _opt.MaxErrorMessageLength
            ? message[.._opt.MaxErrorMessageLength]
            : message;
    }

    private static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
        => value < min ? min : (value > max ? max : value);
}
