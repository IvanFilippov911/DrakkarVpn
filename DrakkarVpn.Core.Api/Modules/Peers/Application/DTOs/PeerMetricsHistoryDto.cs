namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerMetricsHistoryDto(
    DateTime PeriodStartUtc,
    long     TotalRxBytes,
    long     TotalTxBytes,
    bool     IsOnline,
    double?  SpeedMbps,
    double?  VpnLatencyMs
);