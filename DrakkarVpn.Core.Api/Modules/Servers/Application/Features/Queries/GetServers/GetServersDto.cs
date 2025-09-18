namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record GetServersDto(
    Guid Id,
    string Name,
    string Region,
    string PublicHost,
    string Status,
    bool Reachable,
    int PeersActive,
    int? MaxPeers
);