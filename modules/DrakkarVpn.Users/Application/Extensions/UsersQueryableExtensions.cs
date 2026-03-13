using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories.rowDTOs;
using DrakkarVpn.Shared.Subscriptions;
using DrakkarVpn.Users.Infrastructure.EF.Entity;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Extensions;

public static class UsersQueryableExtensions
{
    public static IQueryable<AppUser> ApplySearch(
        this IQueryable<AppUser> query,
        string? search)
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

    public static IQueryable<AppUser> FilterByUserStatus(
        this IQueryable<AppUser> users,
        UserStatus? status)
    {
        if (!status.HasValue)
            return users;

        return status.Value switch
        {
            UserStatus.Active => users.Where(u => u.Status == UserStatus.Active),
            UserStatus.Banned => users.Where(u => u.Status == UserStatus.Banned),
            _                 => users
        };
    }

    
    public static IQueryable<UserWithStatsRow> WithRealtimeStats(
        this IQueryable<AppUser> users,
        IQueryable<UserRealtimeStats> stats)
    {
        return
            from u in users
            join st in stats
                on u.Id equals st.UserId into statsJoin
            from st in statsJoin.DefaultIfEmpty()
            select new UserWithStatsRow
            {
                User  = u,
                Stats = st
            };
    }
    
    public static IQueryable<UserWithStatsRow> FilterBySubscriptionStatus(
        this IQueryable<UserWithStatsRow> query,
        SubscriptionStatus? subStatus,
        DateTime nowUtc)
    {
        if (!subStatus.HasValue)
            return query;

        return subStatus.Value switch
        {
            SubscriptionStatus.Active =>
                query.Where(x => x.Stats != null && x.Stats.IsSubscriptionActive),

            SubscriptionStatus.Expired =>
                query.Where(x =>
                    x.Stats == null ||
                    (!x.Stats.IsSubscriptionActive &&
                     x.Stats.SubscriptionEndUtc != null &&
                     x.Stats.SubscriptionEndUtc <= nowUtc)),

            SubscriptionStatus.Cancelled =>
                query.Where(x =>
                    x.Stats != null &&
                    x.Stats.LastSubscriptionStatus == SubscriptionStatus.Cancelled),

            _ => query
        };
    }

 
    public static IQueryable<UserIndexRowDto> ProjectToUserIndex(
        this IQueryable<UserWithStatsRow> query)
    {
        return query.Select(x =>
            new UserIndexRowDto(
                x.User.Id,
                x.User.TelegramId,
                x.User.CreatedAt,
                x.User.Status,
                x.Stats != null && x.Stats.IsOnline,
                x.Stats != null ? x.Stats.DeviceCount : 0,
                x.Stats != null ? x.Stats.LastSeenUtc : null,
                x.Stats != null && x.Stats.IsSubscriptionActive,
                x.Stats != null ? x.Stats.SubscriptionEndUtc : null,
                x.Stats != null ? x.Stats.SubscriptionMaxDevices : 0,
                x.Stats != null ? x.Stats.Traffic24hBytes : 0L
            )
        );
    }

    public static IQueryable<UserWithStatsRow> ApplySorting(
        this IQueryable<UserWithStatsRow> query,
        UsersSortBy sortBy,
        UserSortDirection direction)
    {
        return (sortBy, direction) switch
        {
            (UsersSortBy.CreatedAt, UserSortDirection.Asc) =>
                query.OrderBy(x => x.User.CreatedAt).ThenBy(x => x.User.Id),

            (UsersSortBy.CreatedAt, UserSortDirection.Desc) =>
                query.OrderByDescending(x => x.User.CreatedAt).ThenBy(x => x.User.Id),

            (UsersSortBy.LastSeen, UserSortDirection.Asc) =>
                query.OrderBy(x => x.Stats!.LastSeenUtc).ThenBy(x => x.User.Id),

            (UsersSortBy.LastSeen, UserSortDirection.Desc) =>
                query.OrderByDescending(x => x.Stats!.LastSeenUtc).ThenBy(x => x.User.Id),

            (UsersSortBy.SubscriptionEnd, UserSortDirection.Asc) =>
                query.OrderBy(x => x.Stats!.SubscriptionEndUtc).ThenBy(x => x.User.Id),

            (UsersSortBy.SubscriptionEnd, UserSortDirection.Desc) =>
                query.OrderByDescending(x => x.Stats!.SubscriptionEndUtc).ThenBy(x => x.User.Id),

            (UsersSortBy.Devices, UserSortDirection.Asc) =>
                query.OrderBy(x => x.Stats!.DeviceCount).ThenBy(x => x.User.Id),

            (UsersSortBy.Devices, UserSortDirection.Desc) =>
                query.OrderByDescending(x => x.Stats!.DeviceCount).ThenBy(x => x.User.Id),

            (UsersSortBy.Traffic24h, UserSortDirection.Asc) =>
                query.OrderBy(x => x.Stats!.Traffic24hBytes).ThenBy(x => x.User.Id),

            (UsersSortBy.Traffic24h, UserSortDirection.Desc) =>
                query.OrderByDescending(x => x.Stats!.Traffic24hBytes).ThenBy(x => x.User.Id),

            _ =>
                query.OrderByDescending(x => x.User.CreatedAt).ThenBy(x => x.User.Id)
        };
    }
}