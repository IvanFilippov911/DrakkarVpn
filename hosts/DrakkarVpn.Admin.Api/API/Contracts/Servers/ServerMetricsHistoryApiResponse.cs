namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;

/// <summary>
/// API response contract for a single server metrics history point.
/// Mirrors ServerMetricsHistoryDto from servers application layer.
/// </summary>
public sealed record ServerMetricsHistoryApiResponse(
    DateTime PeriodStartUtc,
    Guid     ServerId,
    bool     Reachable,
    long     TrafficRxBytes,
    long     TrafficTxBytes,
    decimal  VpnSpeedMbps,
    decimal  InfraLatencyMs);

