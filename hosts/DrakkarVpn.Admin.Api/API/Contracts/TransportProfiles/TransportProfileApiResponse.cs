using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Admin.Api.API.Contracts.TransportProfiles;

public sealed record TransportProfileApiResponse(
    Guid Id,
    string Name,
    TransportType TransportType,
    SecurityType SecurityType,
    string? RealitySni,
    string? RealityShortId,
    string? RealityFingerprint,
    string? RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority,
    int GlobalPriority,
    bool IsEnabled,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    int Version
);
