using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Utils;

public static class ServerPeersQueryUtils
{
    public static IEnumerable<ServerPeerDto> ApplyFilters(
        IEnumerable<ServerPeerDto> src,
        bool onlyOnline,
        Guid? peerId)
    {
        if (onlyOnline)
            src = src.Where(x => x.IsOnline);

        if (peerId is not null && peerId.Value != Guid.Empty)
            src = src.Where(x => x.PeerId == peerId.Value);

        return src;
    }

    public static IEnumerable<ServerPeerDto> ApplySorting(
        IEnumerable<ServerPeerDto> src,
        ServerPeerSortBy? sortBy,
        SortDirection? direction)
    {
        var actualSort = sortBy ?? ServerPeerSortBy.LastActivity;
        var desc       = direction is null || direction == SortDirection.Desc;

        return actualSort switch
        {
            ServerPeerSortBy.LastActivity =>
                OrderNullLast(src, x => x.LastDataAtUtc, desc),

            ServerPeerSortBy.Traffic1h =>
                Order(src, x => x.TrafficLast1hBytes, desc),

            ServerPeerSortBy.Traffic24h =>
                Order(src, x => x.TrafficLast24hBytes, desc),

            ServerPeerSortBy.Speed =>
                OrderNullLast(src, x => x.SpeedMbps, desc),

            ServerPeerSortBy.Latency =>
                OrderNullLast(src, x => x.VpnLatencyMs, desc),

            ServerPeerSortBy.CreatedAt =>
                Order(src, x => x.CreatedAtUtc, desc),

            _ =>
                OrderNullLast(src, x => x.LastDataAtUtc, true)
        };
    }

    private static IOrderedEnumerable<T> Order<T, TKey>(
        IEnumerable<T> src,
        Func<T, TKey> key,
        bool desc)
        => desc ? src.OrderByDescending(key)
                : src.OrderBy(key);

    private static IOrderedEnumerable<T> OrderNullLast<T, TKey>(
        IEnumerable<T> src,
        Func<T, TKey?> key,
        bool desc)
        where TKey : struct, IComparable<TKey>
    {
        return desc
            ? src.OrderBy(x => key(x) is null)
                 .ThenByDescending(x => key(x))
            : src.OrderBy(x => key(x) is null)
                 .ThenBy(x => key(x));
    }
}