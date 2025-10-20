namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record GetServerDto(
    Guid Id,
    string Name,
    string Region,
    string Status,
    bool Reachable,
    int PeersActive,
    int? MaxPeers,
    double VpnSpeedMbps,
    double InfraLatencyMs
);