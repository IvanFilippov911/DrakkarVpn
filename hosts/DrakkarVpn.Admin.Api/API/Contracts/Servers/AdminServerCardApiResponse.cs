namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;

/// <summary>
/// API response contract for a single server in the admin servers list.
/// </summary>
public sealed record AdminServerCardApiResponse(
    Guid   Id,
    string Name,
    string Region,
    string Status,
    bool   Reachable,
    int?   OnlinePeers,
    int    PeersActive,
    int?   MaxPeers,
    double VpnSpeedMbps,
    double InfraLatencyMs,
    long   TrafficLast1hBytes,
    long   TrafficLast24hBytes);

