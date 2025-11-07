namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record AdminOverviewDto(
    int TotalServers,
    int ServersOnline,
    int ActivePeersNow,
    decimal? AvgVpnSpeedMbps,
    decimal? AvgInfraLatencyMs,
    long TrafficTodayBytes,
    long TrafficLast24hBytes,
    int TotalUsers,
    int ActiveSubscriptions,
    int ExpiringSoonDays3,
    int OnlineUsersNow
);