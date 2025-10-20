namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record GetServersDetailDto
(
    Guid Id,
    string Name,
    string Region,
    string PublicHost,
    string AgentBaseUrl,
    string Status,
    bool Reachable,
    int PeersActive,
    int? MaxPeers,
    long TrafficRxBytes,
    long TrafficTxBytes,
    double VpnSpeedMbps,
    double InfraLatencyMs
);