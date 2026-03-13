namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record TrafficOverviewDto(
    long     TrafficTodayBytes,
    long     TrafficLast24hBytes,
    decimal? AvgSpeedMbps,
    decimal? AvgInfraLatencyMs
);