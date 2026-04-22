using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record CreateTransportProfileDto(
    string Name,
    TransportType TransportType,
    SecurityType SecurityType,
    string RealitySni,
    string RealityShortId,
    string RealityFingerprint,
    string RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority,
    int GlobalPriority
);
