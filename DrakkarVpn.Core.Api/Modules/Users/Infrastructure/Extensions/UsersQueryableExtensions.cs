using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public sealed class UserEndRow
{
    public AppUser User { get; init; } = default!;
    public DateTime? EndAtUtc { get; init; }
}

public static class UsersQueryableExtensions
{
    public static IQueryable<AppUser> WhereUserBelongsToServer(
        this IQueryable<AppUser> users, AppDbContext db, Guid serverId)
    {
        var q =
            from p in db.Peers.AsNoTracking()
            where p.ServerId == serverId
            join d in db.Devices.AsNoTracking() on p.DeviceId equals d.DeviceId
            join s in db.Subscriptions.AsNoTracking() on d.SubscriptionId equals s.Id
            where s.Status == SubscriptionStatus.Active
            join u in users on s.UserId equals u.Id
            select u;

        return q.Distinct();
    }

    public static IQueryable<AppUser> ApplySearch(this IQueryable<AppUser> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var raw = search.Trim();

        if (Guid.TryParse(raw, out var userId))
            return query.Where(u => u.Id == userId);

        if (long.TryParse(raw, out var tgId))
            return query.Where(u => u.TelegramId == tgId);

        return query;
    }

    public static IQueryable<AppUser> FilterByUserStatus(this IQueryable<AppUser> users, UserStatus? status)
    {
        if (!status.HasValue) return users;
        return status.Value switch
        {
            UserStatus.Active => users.Where(u => u.Status == UserStatus.Active),
            UserStatus.Banned => users.Where(u => u.Status == UserStatus.Banned),
            _                 => users
        };
    }

    public static IQueryable<UserEndRow> WithMaxEndAndFilter(
        this IQueryable<AppUser> users,
        IQueryable<Subscription> subs,
        SubscriptionStatus? subStatus,
        DateTime nowUtc)
    {
        var maxEndPerUser =
            from s in subs
            group s by s.UserId into g
            select new { UserId = g.Key, EndAt = (DateTime?)g.Max(x => x.EndAt) };

        var joined =
            from u in users
            join me in maxEndPerUser on u.Id equals me.UserId into gj
            from me in gj.DefaultIfEmpty()
            select new UserEndRow { User = u, EndAtUtc = me.EndAt };
        
        if (subStatus.HasValue)
        {
            joined = subStatus == SubscriptionStatus.Active
                ? joined.Where(x => x.EndAtUtc != null && x.EndAtUtc > nowUtc)
                : joined.Where(x => x.EndAtUtc == null || x.EndAtUtc <= nowUtc);
        }

        return joined;
    }

    public static IOrderedQueryable<UserEndRow> SortBy(
        this IQueryable<UserEndRow> query, UsersSortBy sortBy)
        => sortBy switch
        {
            UsersSortBy.EndAtAsc          => query.OrderBy(x => x.EndAtUtc).ThenBy(x => x.User.Id),
            UsersSortBy.EndAtDesc         => query.OrderByDescending(x => x.EndAtUtc).ThenBy(x => x.User.Id),
            UsersSortBy.CreatedAtAsc      => query.OrderBy(x => x.User.CreatedAt).ThenBy(x => x.User.Id),
            UsersSortBy.CreatedAtDesc     => query.OrderByDescending(x => x.User.CreatedAt).ThenBy(x => x.User.Id),
            UsersSortBy.ActiveSubsDesc    => query.OrderByDescending(x => x.EndAtUtc ?? DateTime.MinValue).ThenBy(x => x.User.Id),
            _                             => query.OrderByDescending(x => x.EndAtUtc ?? DateTime.MinValue).ThenBy(x => x.User.Id)
        };

    public static IQueryable<UserIndexRowDto> SelectProjection(
        this IQueryable<UserEndRow> query,
        AppDbContext db,
        Guid serverId)
    {
        return query.Select(x => new UserIndexRowDto(
            x.User.Id,
            x.User.TelegramId,
            x.User.CreatedAt,
            x.User.Status,
            (
                from p in db.Peers
                join d in db.Devices      on p.DeviceId equals d.DeviceId
                join s in db.Subscriptions on d.SubscriptionId equals s.Id
                where p.ServerId == serverId
                      && p.IsOnline
                      && s.UserId == x.User.Id
                select p.Id
            ).Count(),
            (
                from d in db.Devices
                join s in db.Subscriptions on d.SubscriptionId equals s.Id
                where s.UserId == x.User.Id
                select d.DeviceId
            ).Distinct().Count(),
            (
                from p in db.Peers
                join d in db.Devices      on p.DeviceId     equals d.DeviceId
                join s in db.Subscriptions on d.SubscriptionId equals s.Id
                where p.ServerId == serverId
                      && s.UserId == x.User.Id
                      && p.LastDataAt != null
                select p.LastDataAt
            ).Max()
        ));
    }
}