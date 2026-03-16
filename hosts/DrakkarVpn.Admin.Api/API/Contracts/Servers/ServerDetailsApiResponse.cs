namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;

/// <summary>
/// API response contract for server details endpoint.
/// Mirrors GetServersDetailDto from servers application layer.
/// </summary>
public sealed record ServerDetailsApiResponse(
    Guid   Id,
    string Name,
    string Region,
    string PublicHost,
    string AgentBaseUrl,
    string Status,
    bool   Reachable,
    int    PeersActive,
    int?   MaxPeers,
    long   TrafficRxBytes,
    long   TrafficTxBytes,
    double VpnSpeedMbps,
    double InfraLatencyMs);

