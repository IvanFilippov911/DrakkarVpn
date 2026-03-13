namespace DrakkarVpn.Shared;

public sealed record AgentMetricsDto(
    bool Reachable = false,
    int PeersActive = 0,
    long TrafficRxBytes = 0,
    long TrafficTxBytes = 0,
    double InfraLatencyMs = 0,
    double VpnSpeedMbps = 0
)
{
    public static readonly AgentMetricsDto Empty = new();
}