using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Admin.Api.API.Contracts.TransportProfiles;

public sealed record CreateTransportProfileApiRequest(
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
