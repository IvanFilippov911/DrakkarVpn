using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Shared.Peers;

public sealed record PeerBriefDto(
    Guid Id,
    Guid AgentPeerId,
    PeerStatus Status,
    DateTime? LastDataAt,
    long TotalRxBytes,
    long TotalTxBytes,
    double? SpeedMbps,
    DateTime? LastPolledAt,
    double? VpnLatencyMs,
    bool IsOnline   
);