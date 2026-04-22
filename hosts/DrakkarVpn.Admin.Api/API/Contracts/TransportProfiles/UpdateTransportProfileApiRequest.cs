namespace DrakkarVpn.Admin.Api.API.Contracts.TransportProfiles;

public sealed record UpdateTransportProfileApiRequest(
    string Name,
    string RealitySni,
    string RealityShortId,
    string RealityFingerprint,
    string RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority,
    int GlobalPriority
);
