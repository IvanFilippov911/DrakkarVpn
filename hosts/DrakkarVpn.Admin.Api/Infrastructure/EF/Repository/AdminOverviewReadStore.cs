using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Shared.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Admin.Api.Infrastructure.EF.Repository;

public sealed class AdminOverviewReadStore : IAdminOverviewReadStore
{
    private readonly AdminReadDbContext _db;

    public AdminOverviewReadStore(AdminReadDbContext db)
    {
        _db = db;
    }

    public async Task<ServersOverviewDto> GetServersOverviewAsync(CancellationToken ct)
    {
        var totalServers = await _db.Servers.CountAsync(ct);

        var serversOnline = await _db.Servers.CountAsync(
            s => s.Status == ServerStatus.Enabled && s.HealthReachable,
            ct);

        var totalActivePeers = await _db.Peers.CountAsync(
            p => p.Status == (short)PeerStatus.Active,
            ct);

        var peersOnline = await _db.Peers.CountAsync(
            p => p.Status == (short)PeerStatus.Active && p.IsOnline,
            ct);

        return new ServersOverviewDto(
            TotalServers:     totalServers,
            ServersOnline:    serversOnline,
            TotalActivePeers: totalActivePeers,
            PeersOnline:      peersOnline
        );
    }

    public async Task<UsersOverviewDto> GetUsersOverviewAsync(CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;

        var totalUsers = await _db.Users.CountAsync(ct);

        var activeSubscriptions = await _db.Subscriptions.CountAsync(
            s => s.Status == SubscriptionStatus.Active && s.EndAtUtc > nowUtc,
            ct);

        var expiringSoonDays3 = await _db.Subscriptions.CountAsync(
            s => s.Status == SubscriptionStatus.Active &&
                 s.EndAtUtc > nowUtc &&
                 s.EndAtUtc <= nowUtc.AddDays(3),
            ct);

        var onlineUsersNow = await (
                from p in _db.Peers
                where p.Status == (short)PeerStatus.Active && p.IsOnline
                join d in _db.Devices on p.DeviceId equals d.DeviceId
                select d.UserId
            )
            .Distinct()
            .CountAsync(ct);

        return new UsersOverviewDto(
            TotalUsers:          totalUsers,
            ActiveSubscriptions: activeSubscriptions,
            ExpiringSoonDays3:   expiringSoonDays3,
            OnlineUsersNow:      onlineUsersNow
        );
    }

    public async Task<TrafficOverviewDto> GetTrafficOverviewAsync(CancellationToken ct)
    {
        var nowUtc     = DateTime.UtcNow;
        var todayStart = nowUtc.Date;
        var since24h   = nowUtc.AddHours(-24);

        var stats = await _db.ServerMetricsHistories
            .AsNoTracking()
            .Where(h => h.PeriodStartUtc >= since24h)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TrafficLast24h = g.Sum(h => h.TrafficRxDeltaBytes + h.TrafficTxDeltaBytes),
                TrafficToday   = g.Sum(h =>
                    h.PeriodStartUtc >= todayStart
                        ? h.TrafficRxDeltaBytes + h.TrafficTxDeltaBytes
                        : 0),
                AvgSpeed   = g.Average(h => (decimal?)h.VpnSpeedMbps),
                AvgLatency = g.Average(h => (decimal?)h.InfraLatencyMs)
            })
            .FirstOrDefaultAsync(ct);

        if (stats is null)
            return new TrafficOverviewDto(0, 0, null, null);

        return new TrafficOverviewDto(
            TrafficTodayBytes:   stats.TrafficToday,
            TrafficLast24hBytes: stats.TrafficLast24h,
            AvgSpeedMbps:        stats.AvgSpeed,
            AvgInfraLatencyMs:   stats.AvgLatency
        );
    }
}