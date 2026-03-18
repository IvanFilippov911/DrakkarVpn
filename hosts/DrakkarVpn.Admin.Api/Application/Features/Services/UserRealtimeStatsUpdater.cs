using DrakkarVpn.Admin.Api.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;
using DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;
using DrakkarVpn.Shared.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Admin.Api.Application.Features.Services;

public sealed class UserRealtimeStatsUpdater : IUserRealtimeStatsUpdater
{
    private readonly AdminReadDbContext _db;
    private readonly ILogger<UserRealtimeStatsUpdater> _logger;

    private const int BatchSize = 50;

    public UserRealtimeStatsUpdater(
        AdminReadDbContext db,
        ILogger<UserRealtimeStatsUpdater> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task UpdateAllUsersAsync(CancellationToken ct)
    {
        var totalUsers = await _db.Users.CountAsync(ct);
        if (totalUsers == 0)
            return;

        var now = DateTime.UtcNow;

        for (var skip = 0; skip < totalUsers; skip += BatchSize)
        {
            var userIds = await _db.Users
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(BatchSize)
                .Select(x => x.Id)
                .ToListAsync(ct);

            var statsBatch = await BuildStatsBatchAsync(userIds, now, ct);

            await UpsertStatsAsync(statsBatch, ct);
        }
    }

    
    private async Task<List<AdminUserRealtimeStatsReadEntity>> BuildStatsBatchAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var devices = await _db.Devices
            .Where(d => userIds.Contains(d.UserId))
            .GroupBy(d => d.UserId)
            .Select(g => new
            {
                UserId      = g.Key,
                DeviceCount = g.Count(),
                LastSeenUtc = g.Max(x => x.LastSeenUtc)
            })
            .ToListAsync(ct);
        
        var peers = await (
            from d in _db.Devices
            join p in _db.Peers on d.DeviceId equals p.DeviceId into peersJoin
            from p in peersJoin.DefaultIfEmpty()
            join agg in _db.AdminPeerTrafficAggs on p.Id equals agg.PeerId into aggsJoin
            from agg in aggsJoin.DefaultIfEmpty()
            where userIds.Contains(d.UserId)
            group new { d, p, agg } by d.UserId
            into g
            select new
            {
                UserId    = g.Key,
                IsOnline  = g.Any(x => x.p != null && x.p.IsOnline),
                Traffic24 = g.Sum(x => (long?)(x.agg != null ? x.agg.Last24hBytes : 0)) ?? 0L
            }
        ).ToListAsync(ct);
        
        var subs = await _db.Subscriptions
            .Where(s => userIds.Contains(s.UserId))
            .GroupBy(s => s.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                EndAt  = g.Max(x => x.EndAtUtc),
                MaxDevices = g.OrderByDescending(x => x.EndAtUtc)
                    .Select(x => x.MaxDevices)
                    .First(),
                Status = g.OrderByDescending(x => x.EndAtUtc)
                    .Select(x => x.Status)
                    .First()
            })
            .ToListAsync(ct);

        var result = new List<AdminUserRealtimeStatsReadEntity>(userIds.Count);

        foreach (var userId in userIds)
        {
            var dev = devices.FirstOrDefault(x => x.UserId == userId);
            var pr  = peers.FirstOrDefault(x => x.UserId == userId);
            var sub = subs.FirstOrDefault(x => x.UserId == userId);

            result.Add(new AdminUserRealtimeStatsReadEntity
            {
                UserId                  = userId,
                DeviceCount             = dev?.DeviceCount ?? 0,
                LastSeenUtc             = dev?.LastSeenUtc,
                IsOnline                = pr?.IsOnline ?? false,
                Traffic24hBytes         = pr?.Traffic24 ?? 0,

                SubscriptionEndUtc      = sub?.EndAt,
                SubscriptionMaxDevices  = sub?.MaxDevices ?? 0,
                LastSubscriptionStatus  = sub?.Status,
                IsSubscriptionActive    = sub.Status == SubscriptionStatus.Active
                                          && sub.EndAt > nowUtc,

                UpdatedAtUtc            = nowUtc
            });
        }

        return result;
    }

    private async Task UpsertStatsAsync(
        IReadOnlyCollection<AdminUserRealtimeStatsReadEntity> stats,
        CancellationToken ct)
    {
        if (stats.Count == 0)
            return;

        var ids = stats.Select(s => s.UserId).ToList();

        var existingIds = await _db.AdminUsersRealtimeStats
            .Where(x => ids.Contains(x.UserId))
            .Select(x => x.UserId)
            .ToListAsync(ct);

        var toUpdate = stats
            .Where(s => existingIds.Contains(s.UserId))
            .ToList();

        var toInsert = stats
            .Where(s => !existingIds.Contains(s.UserId))
            .ToList();

        if (toInsert.Count > 0)
            await _db.AdminUsersRealtimeStats.AddRangeAsync(toInsert, ct);

        if (toUpdate.Count > 0)
            _db.AdminUsersRealtimeStats.UpdateRange(toUpdate);
    }
}