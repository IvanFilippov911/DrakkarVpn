using DrakkarVpn.Core.Api.Modules.Servers.Domain.Abstractions;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Servers.Domain.Policies;

namespace DrakkarVpn.Servers.Domain.Aggregates;

public sealed class TransportProfile : IAggregateRoot
{
    public const int NameMaxLength = 200;
    public const int GrpcFieldMaxLength = 256;
    public const int RealitySniMaxLength = 255;
    public const int RealityShortIdMaxLength = 64;
    public const int RealityFingerprintMaxLength = 128;
    public const int RealityDestMaxLength = 512;

    private TransportProfile() { }

    private TransportProfile(
        Guid id,
        string name,
        TransportType transportType,
        SecurityType securityType,
        string? realitySni,
        string? realityShortId,
        string? realityFingerprint,
        string? realityDest,
        string? grpcServiceName,
        string? grpcAuthority,
        int globalPriority,
        bool isEnabled,
        DateTime createdAtUtc,
        DateTime updatedAtUtc,
        int version)
    {
        Id = id;
        Name = name;
        TransportType = transportType;
        SecurityType = securityType;
        RealitySni = realitySni;
        RealityShortId = realityShortId;
        RealityFingerprint = realityFingerprint;
        RealityDest = realityDest;
        GrpcServiceName = grpcServiceName;
        GrpcAuthority = grpcAuthority;
        GlobalPriority = globalPriority;
        IsEnabled = isEnabled;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        Version = version;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;

    public TransportType TransportType { get; private set; }
    public SecurityType SecurityType { get; private set; }

    public string? RealitySni { get; private set; }
    public string? RealityShortId { get; private set; }
    public string? RealityFingerprint { get; private set; }
    public string? RealityDest { get; private set; }

    public string? GrpcServiceName { get; private set; }
    public string? GrpcAuthority { get; private set; }

    public int GlobalPriority { get; private set; }
    public bool IsEnabled { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public int Version { get; private set; }

    public static TransportProfile CreateRealityTcp(
        Guid id,
        string name,
        int globalPriority,
        string realitySni,
        string realityShortId,
        string realityFingerprint,
        string realityDest,
        DateTime createdAtUtc)
        => CreateRealityCore(
            id,
            name,
            TransportType.Tcp,
            globalPriority,
            realitySni,
            realityShortId,
            realityFingerprint,
            realityDest,
            grpcServiceName: null,
            grpcAuthority: null,
            createdAtUtc);

    public static TransportProfile CreateRealityGrpc(
        Guid id,
        string name,
        int globalPriority,
        string realitySni,
        string realityShortId,
        string realityFingerprint,
        string realityDest,
        string grpcServiceName,
        string? grpcAuthority,
        DateTime createdAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(grpcServiceName);

        return CreateRealityCore(
            id,
            name,
            TransportType.Grpc,
            globalPriority,
            realitySni,
            realityShortId,
            realityFingerprint,
            realityDest,
            NormalizeRequired(grpcServiceName, nameof(grpcServiceName), GrpcFieldMaxLength),
            NormalizeOptional(grpcAuthority, nameof(grpcAuthority), GrpcFieldMaxLength),
            createdAtUtc);
    }

    public void UpdateName(string name, DateTime utcNow)
    {
        Name = NormalizeRequired(name, nameof(name), NameMaxLength);
        Touch(utcNow);
    }

    public void UpdatePriority(int globalPriority, DateTime utcNow)
    {
        ValidatePriority(globalPriority);
        GlobalPriority = globalPriority;
        Touch(utcNow);
    }

    public void UpdateRealitySettings(
        string realitySni,
        string realityShortId,
        string realityFingerprint,
        string realityDest,
        DateTime utcNow)
    {
        TransportProfileSecurityInvariants.EnsureRealitySettingsMutationAllowed(SecurityType);

        RealitySni = NormalizeRequired(realitySni, nameof(realitySni), RealitySniMaxLength);
        RealityShortId = NormalizeRequired(realityShortId, nameof(realityShortId), RealityShortIdMaxLength);
        RealityFingerprint = NormalizeRequired(realityFingerprint, nameof(realityFingerprint), RealityFingerprintMaxLength);
        RealityDest = NormalizeRequired(realityDest, nameof(realityDest), RealityDestMaxLength);

        Touch(utcNow);
    }

    public void UpdateGrpcSettings(string? grpcServiceName, string? grpcAuthority, DateTime utcNow)
    {
        if (TransportType != TransportType.Grpc)
        {
            if (!string.IsNullOrWhiteSpace(grpcServiceName) || !string.IsNullOrWhiteSpace(grpcAuthority))
                throw new InvalidOperationException("gRPC settings are only applicable when transport type is Grpc.");

            GrpcServiceName = null;
            GrpcAuthority = null;
            Touch(utcNow);
            return;
        }

        GrpcServiceName = NormalizeRequired(grpcServiceName!, nameof(grpcServiceName), GrpcFieldMaxLength);
        GrpcAuthority = NormalizeOptional(grpcAuthority, nameof(grpcAuthority), GrpcFieldMaxLength);
        Touch(utcNow);
    }

    public void Update(
        string name,
        int globalPriority,
        string realitySni,
        string realityShortId,
        string realityFingerprint,
        string realityDest,
        string? grpcServiceName,
        string? grpcAuthority,
        DateTime utcNow)
    {
        Name = NormalizeRequired(name, nameof(name), NameMaxLength);
        ValidatePriority(globalPriority);
        GlobalPriority = globalPriority;

        TransportProfileSecurityInvariants.EnsureRealitySettingsMutationAllowed(SecurityType);
        RealitySni = NormalizeRequired(realitySni, nameof(realitySni), RealitySniMaxLength);
        RealityShortId = NormalizeRequired(realityShortId, nameof(realityShortId), RealityShortIdMaxLength);
        RealityFingerprint = NormalizeRequired(realityFingerprint, nameof(realityFingerprint), RealityFingerprintMaxLength);
        RealityDest = NormalizeRequired(realityDest, nameof(realityDest), RealityDestMaxLength);

        if (TransportType == TransportType.Grpc)
        {
            GrpcServiceName = NormalizeRequired(grpcServiceName!, nameof(grpcServiceName), GrpcFieldMaxLength);
            GrpcAuthority = NormalizeOptional(grpcAuthority, nameof(grpcAuthority), GrpcFieldMaxLength);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(grpcServiceName) || !string.IsNullOrWhiteSpace(grpcAuthority))
                throw new InvalidOperationException("gRPC settings are only applicable when transport type is Grpc.");

            GrpcServiceName = null;
            GrpcAuthority = null;
        }

        Touch(utcNow);
    }

    public void Enable(DateTime utcNow)
    {
        if (IsEnabled)
            return;

        IsEnabled = true;
        Touch(utcNow);
    }

    public void Disable(DateTime utcNow)
    {
        if (!IsEnabled)
            return;

        IsEnabled = false;
        Touch(utcNow);
    }

    private static TransportProfile CreateRealityCore(
        Guid id,
        string name,
        TransportType transportType,
        int globalPriority,
        string realitySni,
        string realityShortId,
        string realityFingerprint,
        string realityDest,
        string? grpcServiceName,
        string? grpcAuthority,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must not be empty.", nameof(id));

        ValidatePriority(globalPriority);
        createdAtUtc = EnsureUtc(createdAtUtc, nameof(createdAtUtc));

        var normalizedName = NormalizeRequired(name, nameof(name), NameMaxLength);
        var rsni = NormalizeRequired(realitySni, nameof(realitySni), RealitySniMaxLength);
        var rshort = NormalizeRequired(realityShortId, nameof(realityShortId), RealityShortIdMaxLength);
        var rfinger = NormalizeRequired(realityFingerprint, nameof(realityFingerprint), RealityFingerprintMaxLength);
        var rdest = NormalizeRequired(realityDest, nameof(realityDest), RealityDestMaxLength);

        TransportProfileSecurityInvariants.EnsureRealityFactorySecurity(SecurityType.Reality);

        if (transportType == TransportType.Grpc)
        {
            if (string.IsNullOrWhiteSpace(grpcServiceName))
                throw new ArgumentNullException(nameof(grpcServiceName), "GrpcServiceName is required for Grpc transport.");
        }
        else if (grpcServiceName is not null || grpcAuthority is not null)
        {
            throw new ArgumentException("gRPC fields must be null for non-Grpc transport.");
        }

        return new TransportProfile(
            id,
            normalizedName,
            transportType,
            SecurityType.Reality,
            rsni,
            rshort,
            rfinger,
            rdest,
            grpcServiceName,
            grpcAuthority,
            globalPriority,
            isEnabled: true,
            createdAtUtc,
            createdAtUtc,
            version: 1);
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

    private static void ValidatePriority(int priority)
    {
        if (priority < 0)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be non-negative.");
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

    private static string? NormalizeOptional(string? value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

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