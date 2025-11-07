using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;

public sealed class GetAdminOverviewHandler
    : IRequestHandler<GetAdminOverviewQuery, AdminOverviewDto>
{
    private readonly AppDbContext _db;

    public GetAdminOverviewHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AdminOverviewDto> Handle(GetAdminOverviewQuery q, CancellationToken ct)
    {
        var nowUtc     = DateTime.UtcNow;
        var todayStart = nowUtc.Date;
        var since24h   = nowUtc.AddHours(-24);

        var serversStats = await LoadServersStats(ct);
        var usersStats   = await LoadUsersStats(nowUtc, ct);
        var trafficStats = await LoadTrafficStats(todayStart, since24h, ct);

        return new AdminOverviewDto(
            TotalServers:         serversStats.TotalServers,
            ServersOnline:        serversStats.ServersOnline,
            ActivePeersNow:       serversStats.ActivePeersNow,
            AvgVpnSpeedMbps:      trafficStats.AvgSpeedMbps,
            AvgInfraLatencyMs:    trafficStats.AvgInfraLatencyMs,
            TrafficTodayBytes:    trafficStats.TrafficTodayBytes,
            TrafficLast24hBytes:  trafficStats.TrafficLast24hBytes,
            TotalUsers:           usersStats.TotalUsers,
            ActiveSubscriptions:  usersStats.ActiveSubscriptions,
            ExpiringSoonDays3:    usersStats.ExpiringSoonDays3,
            OnlineUsersNow:       usersStats.OnlineUsersNow
        );
    }
    

    private sealed record ServersStats(
        int TotalServers,
        int ServersOnline,
        int ActivePeersNow
    );

    private sealed record UsersStats(
        int TotalUsers,
        int ActiveSubscriptions,
        int ExpiringSoonDays3,
        int OnlineUsersNow
    );

    private sealed record TrafficStats(
        long    TrafficTodayBytes,
        long    TrafficLast24hBytes,
        decimal? AvgSpeedMbps,
        decimal? AvgInfraLatencyMs
    );
    

    private async Task<ServersStats> LoadServersStats(CancellationToken ct)
    {
        var totalServers = await _db.Servers.CountAsync(ct);

        var serversOnline = await _db.Servers
            .CountAsync(
                s => s.Status == ServerStatus.Enabled &&
                     s.Health.Reachable,
                ct);

        var activePeersNow = await _db.Peers
            .CountAsync(
                p => p.Status == PeerStatus.Active &&
                     p.IsOnline,
                ct);

        return new ServersStats(
            TotalServers:  totalServers,
            ServersOnline: serversOnline,
            ActivePeersNow: activePeersNow
        );
    }
    

    private async Task<UsersStats> LoadUsersStats(DateTime nowUtc, CancellationToken ct)
    {
        var totalUsers = await _db.Users.CountAsync(ct);

        var activeSubscriptions = await _db.Subscriptions
            .CountAsync(
                s => s.Status == SubscriptionStatus.Active &&
                     s.EndAt > nowUtc,
                ct);

        var expiringSoonDays3 = await _db.Subscriptions
            .CountAsync(
                s => s.Status == SubscriptionStatus.Active &&
                     s.EndAt > nowUtc &&
                     s.EndAt <= nowUtc.AddDays(3),
                ct);
        
        var onlineUsersNow = await (
            from p in _db.Peers
            where p.Status == PeerStatus.Active && p.IsOnline
            join d in _db.Devices       on p.DeviceId equals d.DeviceId
            join s in _db.Subscriptions on d.SubscriptionId equals s.Id
            where s.Status == SubscriptionStatus.Active && s.EndAt > nowUtc
            select s.UserId
        ).Distinct().CountAsync(ct);

        return new UsersStats(
            TotalUsers:          totalUsers,
            ActiveSubscriptions: activeSubscriptions,
            ExpiringSoonDays3:   expiringSoonDays3,
            OnlineUsersNow:      onlineUsersNow
        );
    }
    

    private async Task<TrafficStats> LoadTrafficStats(
        DateTime todayStart,
        DateTime since24h,
        CancellationToken ct)
    {
        var recentServerHistory = await _db.ServerMetricsHistory
            .Where(h => h.PeriodStartUtc >= since24h)
            .ToListAsync(ct);

        if (recentServerHistory.Count == 0)
        {
            return new TrafficStats(
                TrafficTodayBytes:   0,
                TrafficLast24hBytes: 0,
                AvgSpeedMbps:        null,
                AvgInfraLatencyMs:   null
            );
        }

        var trafficLast24hBytes = recentServerHistory
            .Sum(h => h.TrafficRxBytes + h.TrafficTxBytes);

        var trafficTodayBytes = recentServerHistory
            .Where(h => h.PeriodStartUtc >= todayStart)
            .Sum(h => h.TrafficRxBytes + h.TrafficTxBytes);

        decimal? avgSpeedMbps = null;
        decimal? avgInfraLatencyMs = null;

        var speeds = recentServerHistory
            .Select(h => h.VpnSpeedMbps)
            .ToList();

        if (speeds.Count > 0)
            avgSpeedMbps = speeds.Average();

        var latencies = recentServerHistory
            .Select(h => h.InfraLatencyMs)
            .ToList();

        if (latencies.Count > 0)
            avgInfraLatencyMs = latencies.Average();

        return new TrafficStats(
            TrafficTodayBytes:   trafficTodayBytes,
            TrafficLast24hBytes: trafficLast24hBytes,
            AvgSpeedMbps:        avgSpeedMbps,
            AvgInfraLatencyMs:   avgInfraLatencyMs
        );
    }
}