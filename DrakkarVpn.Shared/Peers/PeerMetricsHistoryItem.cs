namespace DrakkarVpn.Shared.Peers;

public sealed record PeerMetricsHistoryItem(
    Guid PeerId,
    long TotalRxBytes,
    long TotalTxBytes,
    bool IsOnline,
    double? VpnLatencyMs,
    DateTime? LastDataAt,
    DateTime? LastLatencyAt
);