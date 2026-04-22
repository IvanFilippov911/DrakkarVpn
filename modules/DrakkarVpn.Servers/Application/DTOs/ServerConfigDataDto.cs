using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record ServerConfigDataDto(
    string Region,
    string PublicHost,
    int PublicPort,
    TransportType TransportType,
    SecurityType SecurityType,
    string RealitySni,
    string RealityPublicKey,
    string RealityShortId,
    string RealityFingerprint,
    string? RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority);

