namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record ServerMetricsHistoryDto(
    DateTime PeriodStartUtc,
    Guid ServerId,
    bool Reachable,
    int PeersActive,
    int? MaxPeers,
    long TrafficRxBytes,
    long TrafficTxBytes,
    decimal VpnSpeedMbps,
    decimal InfraLatencyMs
);