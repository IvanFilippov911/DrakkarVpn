namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record AdminServerCardDto(
    Guid   Id,
    string Name,
    string Region,
    string Status,
    bool   Reachable,
    int    PeersActive,
    int?    MaxPeers,
    double VpnSpeedMbps,
    double InfraLatencyMs,
    long   TrafficLast1hBytes,
    long   TrafficLast24hBytes
);