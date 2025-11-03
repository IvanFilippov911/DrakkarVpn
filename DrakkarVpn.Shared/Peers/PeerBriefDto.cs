namespace DrakkarVpn.Shared.Peers;

public sealed record PeerBriefDto(
    Guid Id,
    Guid AgentPeerId,
    short Status,
    DateTime? LastDataAt,
    long TotalRxBytes,
    long TotalTxBytes,
    double? VpnLatencyMs,
    bool IsOnline   
);