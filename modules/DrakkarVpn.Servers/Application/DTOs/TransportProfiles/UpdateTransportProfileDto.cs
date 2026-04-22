namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record UpdateTransportProfileDto(
    string Name,
    string RealitySni,
    string RealityShortId,
    string RealityFingerprint,
    string RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority,
    int GlobalPriority
);
