using DrakkarVpn.Servers.Domain.Enums.TransportProfile;

namespace DrakkarVpn.Servers.Infrastructure.EF.Entities;

public sealed class ServerTransportApplyJob
{
    public Guid JobId { get; private set; }
    public Guid ServerId { get; private set; }
    public Guid ActivationId { get; private set; }

    public long TargetTransportVersion { get; private set; }

    public ServerTransportApplyJobStatus State { get; private set; }

    public DateTime? LeaseUntilUtc { get; private set; }
    public string? LeaseOwner { get; private set; }

    public int Attempt { get; private set; }
    public int MaxAttempt { get; private set; }
    public DateTime NextAttemptUtc { get; private set; }

    public string? LastErrorCode { get; private set; }
    public string? LastErrorMessage { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private ServerTransportApplyJob() { }

    public static ServerTransportApplyJob CreateNew(
        Guid serverId,
        Guid activationId,
        long targetTransportVersion,
        int maxAttempt,
        DateTime utcNow)
    {
        if (serverId == Guid.Empty) throw new ArgumentException("serverId is required", nameof(serverId));
        if (activationId == Guid.Empty) throw new ArgumentException("activationId is required", nameof(activationId));
        if (targetTransportVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetTransportVersion));
        if (maxAttempt <= 0) throw new ArgumentException("maxAttempt must be > 0", nameof(maxAttempt));

        utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

        return new ServerTransportApplyJob
        {
            JobId = Guid.NewGuid(),
            ServerId = serverId,
            ActivationId = activationId,
            TargetTransportVersion = targetTransportVersion,
            State = ServerTransportApplyJobStatus.Pending,
            Attempt = 0,
            MaxAttempt = maxAttempt,
            NextAttemptUtc = utcNow,
            CreatedAtUtc = utcNow,
            UpdatedAtUtc = utcNow
        };
    }
}