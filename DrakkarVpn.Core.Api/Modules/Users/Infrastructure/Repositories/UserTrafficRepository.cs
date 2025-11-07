using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;

public sealed class UserTrafficRepository : IUserTrafficRepository
{
    private readonly AppDbContext _db;

    public UserTrafficRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<Guid, UsersTrafficSummaryDto>> GetTrafficLast24hAsync(
        Guid serverId,
        Guid[] userIds,
        DateTime fromUtc,
        CancellationToken ct)
    {
        if (userIds is null || userIds.Length == 0)
            return new();

        var rows = await (
                from h in _db.PeerMetricsHistory.AsNoTracking()
                join p in _db.Peers.AsNoTracking()        on h.PeerId        equals p.Id
                join d in _db.Devices.AsNoTracking()      on p.DeviceId      equals d.DeviceId
                join s in _db.Subscriptions.AsNoTracking() on d.SubscriptionId equals s.Id
                where p.ServerId == serverId
                      && userIds.Contains(s.UserId)
                      && h.PeriodStartUtc >= fromUtc
                select new
                {
                    s.UserId,
                    h.TotalRxBytes,
                    h.TotalTxBytes
                })
            .ToListAsync(ct);

        return rows
            .GroupBy(r => r.UserId)
            .Select(g => new UsersTrafficSummaryDto(
                UserId: g.Key,
                TrafficLast24hBytes: g.Sum(x => x.TotalRxBytes + x.TotalTxBytes)
            ))
            .ToDictionary(x => x.UserId, x => x);
    }
}