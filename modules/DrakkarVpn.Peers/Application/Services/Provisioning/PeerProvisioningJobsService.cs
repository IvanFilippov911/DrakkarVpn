using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerProvisionJobsService : IPeerProvisionJobsService
{
    private readonly IPeerProvisionJobsRepository _repo;

    private const int DefaultMaxAttempts = 10;
    private const int MaxDeviceIdLen = 128;

    private const int MaxErrorCodeLen = 128;
    private const int MaxErrorMessageLen = 2048;

    private static readonly TimeSpan MinLease = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaxLease = TimeSpan.FromMinutes(5);
    private const int MaxTake = 500;

    public PeerProvisionJobsService(IPeerProvisionJobsRepository repo)
    {
        _repo = repo;
    }

    public Task<Guid> EnqueueAsync(PeerProvisionJobCreateDto dto, DateTime nowUtc, CancellationToken ct)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (dto.UserId == Guid.Empty) throw new ArgumentException("UserId is required", nameof(dto));
        if (dto.ServerId == Guid.Empty) throw new ArgumentException("ServerId is required", nameof(dto));

        var deviceId = NormalizeDeviceId(dto.DeviceId);

        if (dto.AgentPeerUuid == Guid.Empty)
            throw new ArgumentException("AgentPeerUuid is required", nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.ConfigRaw))
            throw new ArgumentException("ConfigRaw is required", nameof(dto));

        nowUtc = EnsureUtc(nowUtc);

        var maxAttempts = dto.MaxAttempts <= 0 ? DefaultMaxAttempts : dto.MaxAttempts;

        return _repo.CreateOrGetAsync(
            dto with { DeviceId = deviceId, MaxAttempts = maxAttempts },
            nowUtc,
            ct);
    }

    public async Task<PeerProvisionJobDto?> GetAsync(Guid jobId, CancellationToken ct)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("jobId is required", nameof(jobId));

        var job = await _repo.GetByIdAsync(jobId, ct);
        if (job is null) return null;

        return new PeerProvisionJobDto(
            JobId: job.JobId,
            State: job.State,
            Attempt: job.Attempt,
            MaxAttempts: job.MaxAttempts,
            NextAttemptAtUtc: job.NextAttemptAtUtc,
            PeerId: job.PeerId,
            AgentPeerUuid: job.AgentPeerUuid,
            ConfigRaw: job.ConfigRaw,
            ErrorCode: job.LastErrorCode,
            ErrorMessage: job.LastErrorMessage
        );
    }

    public Task<IReadOnlyList<PeerProvisionJob>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(instanceId))
            throw new ArgumentException("instanceId is required", nameof(instanceId));

        if (take <= 0)
            return Task.FromResult<IReadOnlyList<PeerProvisionJob>>([]);

        if (take > MaxTake) take = MaxTake;

        nowUtc = EnsureUtc(nowUtc);
        lease = Clamp(lease, MinLease, MaxLease);

        return _repo.AcquireBatchAsync(take, lease, nowUtc, instanceId, ct);
    }

    public Task MarkAgentAppliedAsync(
        Guid jobId,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("jobId is required", nameof(jobId));
        nowUtc = EnsureUtc(nowUtc);

        return _repo.MarkAgentAppliedAsync(jobId, nowUtc, ct);
    }

    public Task MarkReadyAsync(
        Guid jobId,
        Guid peerId,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("jobId is required", nameof(jobId));
        if (peerId == Guid.Empty) throw new ArgumentException("peerId is required", nameof(peerId));

        nowUtc = EnsureUtc(nowUtc);

        return _repo.MarkReadyAsync(jobId, peerId, nowUtc, ct);
    }

    public Task FailOrRescheduleAsync(
        PeerProvisionJob jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime nowUtc,
        Func<int, TimeSpan> backoff,
        CancellationToken ct)
    {
        if (jobSnapshot is null) throw new ArgumentNullException(nameof(jobSnapshot));
        if (jobSnapshot.JobId == Guid.Empty) throw new ArgumentException("JobId is required", nameof(jobSnapshot));
        if (jobSnapshot.MaxAttempts <= 0) throw new ArgumentException("MaxAttempts must be > 0", nameof(jobSnapshot));
        if (backoff is null) throw new ArgumentNullException(nameof(backoff));

        nowUtc = EnsureUtc(nowUtc);

        var code = NormalizeErrorCode(errorCode);
        var msg  = NormalizeErrorMessage(errorMessage);

        var nextAttempt = jobSnapshot.Attempt + 1;
        
        if (nextAttempt >= jobSnapshot.MaxAttempts)
        {
            return _repo.MarkFailedAsync(jobSnapshot.JobId, code, msg, nowUtc, ct);
        }

        var delay = backoff(nextAttempt);
        if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

        var nextAt = nowUtc.Add(delay);

        return _repo.RescheduleAsync(
            jobId: jobSnapshot.JobId,
            newAttempt: nextAttempt,
            nextAttemptAtUtc: nextAt,
            code: code,
            message: msg,
            nowUtc: nowUtc,
            ct: ct);
    }
    
    public Task FailPermanentAsync(
        PeerProvisionJob jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (jobSnapshot is null) throw new ArgumentNullException(nameof(jobSnapshot));
        if (jobSnapshot.JobId == Guid.Empty) throw new ArgumentException("JobId is required", nameof(jobSnapshot));

        nowUtc = EnsureUtc(nowUtc);

        var code = NormalizeErrorCode(errorCode);
        var msg  = NormalizeErrorMessage(errorMessage);

        return _repo.MarkFailedAsync(jobSnapshot.JobId, code, msg, nowUtc, ct);
    }

    private static string NormalizeDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("DeviceId is required", nameof(deviceId));

        deviceId = deviceId.Trim();

        if (deviceId.Length > MaxDeviceIdLen)
            throw new ArgumentException($"DeviceId max length is {MaxDeviceIdLen}", nameof(deviceId));

        return deviceId;
    }

    private static string NormalizeErrorCode(string errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
            errorCode = "Unknown";

        errorCode = errorCode.Trim();

        if (errorCode.Length > MaxErrorCodeLen)
            errorCode = errorCode[..MaxErrorCodeLen];

        return errorCode;
    }

    private static string? NormalizeErrorMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        message = message.Trim();

        if (message.Length > MaxErrorMessageLen)
            message = message[..MaxErrorMessageLen];

        return message;
    }

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    private static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
        => value < min ? min : (value > max ? max : value);
}
