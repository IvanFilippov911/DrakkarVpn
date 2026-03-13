namespace DrakkarVpn.Shared.Peers;

public sealed record PeerMetricsDto(
    Guid AgentPeerId,
    long? RxBytesTotal,
    long? TxBytesTotal,
    double? VpnLatencyMs
);