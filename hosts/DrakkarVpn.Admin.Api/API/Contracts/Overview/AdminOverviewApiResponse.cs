namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Overview;

/// <summary>
/// API response contract for GET /api/admin/overview (dashboard overview).
/// </summary>
public sealed record AdminOverviewApiResponse(
    int TotalServers,
    int ServersOnline,
    int TotalActivePeers,
    int PeersOnline,
    decimal? AvgVpnSpeedMbps,
    decimal? AvgInfraLatencyMs,
    long TrafficTodayBytes,
    long TrafficLast24hBytes,
    int TotalUsers,
    int ActiveSubscriptions,
    int ExpiringSoonDays3,
    int OnlineUsersNow);
