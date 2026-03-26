namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record ServerConfigDataDto(
    string PublicHost,
    int    PublicPort,
    string RealitySni,
    string RealityPublicKey,
    string RealityShortId);

