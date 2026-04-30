using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Servers.Domain.Entities;

public sealed class ServerTransportProfileActivation
{
    public const int RealityPublicKeyMaxLength = 512;

    private ServerTransportProfileActivation() { }

    private ServerTransportProfileActivation(
        Guid id,
        Guid serverId,
        Guid transportProfileId,
        string realityPublicKey,
        int localPriority,
        TransportActivationStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc,
        DateTimeOffset? activatedAtUtc,
        int version)
    {
        Id = id;
        ServerId = serverId;
        TransportProfileId = transportProfileId;
        RealityPublicKey = realityPublicKey;
        LocalPriority = localPriority;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        ActivatedAtUtc = activatedAtUtc;
        Version = version;
    }

    public Guid Id { get; private set; }
    public Guid ServerId { get; private set; }
    public Guid TransportProfileId { get; private set; }

    public string RealityPublicKey { get; private set; } = default!;
    public int LocalPriority { get; private set; }

    public TransportActivationStatus Status { get; private set; }
    public DateTimeOffset? ActivatedAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public int Version { get; private set; }

    public static ServerTransportProfileActivation Create(
        Guid id,
        Guid serverId,
        Guid transportProfileId,
        string realityPublicKey,
        int localPriority,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must not be empty.", nameof(id));
        if (serverId == Guid.Empty)
            throw new ArgumentException("ServerId must not be empty.", nameof(serverId));
        if (transportProfileId == Guid.Empty)
            throw new ArgumentException("TransportProfileId must not be empty.", nameof(transportProfileId));
        if (localPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(localPriority), "LocalPriority must be non-negative.");

        createdAtUtc = EnsureUtc(createdAtUtc, nameof(createdAtUtc));

        return new ServerTransportProfileActivation(
            id,
            serverId,
            transportProfileId,
            NormalizeRequired(realityPublicKey, nameof(realityPublicKey), RealityPublicKeyMaxLength),
            localPriority,
            TransportActivationStatus.Standby,
            createdAtUtc,
            createdAtUtc,
            activatedAtUtc: null,
            version: 1);
    }

    public void Update(string realityPublicKey, int localPriority, DateTime utcNow)
    {
        RealityPublicKey = NormalizeRequired(
            realityPublicKey,
            nameof(realityPublicKey),
            RealityPublicKeyMaxLength);

        if (localPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(localPriority), "LocalPriority must be non-negative.");

        LocalPriority = localPriority;

        Touch(utcNow);
    }

    public void MarkActive(DateTimeOffset nowUtc)
    {
        if (Status == TransportActivationStatus.Active)
            return;

        Status = TransportActivationStatus.Active;
        ActivatedAtUtc = nowUtc.ToUniversalTime();
        Touch(nowUtc.UtcDateTime);
    }

    public void MarkStandby(DateTime utcNow)
    {
        if (Status == TransportActivationStatus.Standby)
            return;

        Status = TransportActivationStatus.Standby;
        ActivatedAtUtc = null;
        Touch(utcNow);
    }

    private void Touch(DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow, nameof(utcNow));
        UpdatedAtUtc = utcNow;
        checked
        {
            Version++;
        }
    }

    private static string NormalizeRequired(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(paramName);

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(paramName, $"Maximum length is {maxLength}.");

        return normalized;
    }

    private static DateTime EnsureUtc(DateTime value, string paramName)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Timestamp must be UTC.", paramName);

        return value;
    }
}