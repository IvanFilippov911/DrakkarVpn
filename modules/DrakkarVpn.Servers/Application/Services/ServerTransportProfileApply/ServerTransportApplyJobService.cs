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

    public Task<Guid> EnqueueAsync(Guid serverId, Guid activationId, CancellationToken ct)
    {
        if (serverId == Guid.Empty) throw new ArgumentException("serverId is required", nameof(serverId));
        if (activationId == Guid.Empty) throw new ArgumentException("activationId is required", nameof(activationId));

        return _repo.CreateOrGetAsync(
            serverId,
            activationId,
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
        ServerTransportApplyJobDto jobSnapshot,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (jobSnapshot is null) throw new ArgumentNullException(nameof(jobSnapshot));
        if (jobSnapshot.JobId == Guid.Empty) throw new ArgumentException("JobId is required", nameof(jobSnapshot));
        if (string.IsNullOrWhiteSpace(jobSnapshot.LeaseOwner))
            throw new ArgumentException("LeaseOwner is required", nameof(jobSnapshot));

        return _repo.MarkCompletedAsync(
            jobSnapshot.JobId,
            jobSnapshot.LeaseOwner,
            EnsureUtc(utcNow),
            ct);
    }

    public Task<int> FailPermanentAsync(
        ServerTransportApplyJobDto jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (jobSnapshot is null) throw new ArgumentNullException(nameof(jobSnapshot));
        if (jobSnapshot.JobId == Guid.Empty) throw new ArgumentException("JobId is required", nameof(jobSnapshot));

        utcNow = EnsureUtc(utcNow);
        var code = NormalizeErrorCode(errorCode);
        var message = NormalizeErrorMessage(errorMessage);

        if (string.IsNullOrWhiteSpace(jobSnapshot.LeaseOwner))
            throw new ArgumentException("LeaseOwner is required", nameof(jobSnapshot));

        return _repo.MarkFailedAsync(
            jobSnapshot.JobId,
            jobSnapshot.LeaseOwner,
            code,
            message,
            utcNow,
            ct);
    }

    public Task<int> FailOrRescheduleAsync(
        ServerTransportApplyJobDto jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime utcNow,
        Func<int, TimeSpan> backoff,
        CancellationToken ct)
    {
        if (jobSnapshot is null) throw new ArgumentNullException(nameof(jobSnapshot));
        if (jobSnapshot.JobId == Guid.Empty) throw new ArgumentException("JobId is required", nameof(jobSnapshot));
        if (jobSnapshot.MaxAttempt <= 0) throw new ArgumentException("MaxAttempt must be > 0", nameof(jobSnapshot));
        if (backoff is null) throw new ArgumentNullException(nameof(backoff));
        if (string.IsNullOrWhiteSpace(jobSnapshot.LeaseOwner))
            throw new ArgumentException("LeaseOwner is required", nameof(jobSnapshot));

        utcNow = EnsureUtc(utcNow);
        var code = NormalizeErrorCode(errorCode);
        var message = NormalizeErrorMessage(errorMessage);
        var nextAttempt = jobSnapshot.Attempt + 1;

        if (nextAttempt >= jobSnapshot.MaxAttempt)
        {
            return _repo.MarkFailedAsync(
                jobSnapshot.JobId,
                jobSnapshot.LeaseOwner,
                code,
                message,
                utcNow,
                ct);
        }

        var delay = backoff(nextAttempt);
        if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

        return _repo.RescheduleAsync(
            jobId: jobSnapshot.JobId,
            leaseOwner: jobSnapshot.LeaseOwner,
            newAttempt: nextAttempt,
            nextAttemptAtUtc: utcNow.Add(delay),
            code: code,
            message: message,
            utcNow: utcNow,
            ct: ct);
    }

    private static ServerTransportApplyJobDto Map(ServerTransportApplyJob row)
        => new(
            row.JobId,
            row.ServerId,
            row.ActivationId,
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