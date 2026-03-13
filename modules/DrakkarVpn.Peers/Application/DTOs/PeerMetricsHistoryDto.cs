namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerMetricsHistoryDto(
    DateTime PeriodStartUtc,
    long     TotalRxDeltaBytes,
    long     TotalTxDeltaBytes,
    bool     IsOnline,
    double?  SpeedMbps,
    double?  VpnLatencyMs
);