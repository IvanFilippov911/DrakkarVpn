namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;

public sealed record AdminServerIndexRowDto(
    Guid Id,
    string Name,
    string Region,
    string Status,
    bool Reachable,
    int PeersActive,
    int? MaxPeers,
    double VpnSpeedMbps,
    double InfraLatencyMs,
    int OnlinePeers,
    long TrafficLast1hBytes,
    long TrafficLast24hBytes
);