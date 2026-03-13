namespace DrakkarVpn.Shared.Peers;

public sealed record PeerMetricsHistoryItem(
    Guid PeerId,
    long TotalRxBytes,
    long TotalTxBytes,
    long RxDeltaBytes,
    long TxDeltaBytes,
    bool IsOnline,
    double? VpnLatencyMs
);