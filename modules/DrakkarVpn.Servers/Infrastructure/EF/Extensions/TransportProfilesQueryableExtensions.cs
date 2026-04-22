using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Shared.Servers;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Servers.Infrastructure.EF.Extensions;

public static class TransportProfilesQueryableExtensions
{
    public static IQueryable<TransportProfile> FilterBySearch(
        this IQueryable<TransportProfile> query,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var pattern = $"%{search.Trim()}%";
        return query.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.ILike(x.Name, pattern));
    }

    public static IQueryable<TransportProfile> FilterByIsEnabled(
        this IQueryable<TransportProfile> query,
        bool? isEnabled)
    {
        if (!isEnabled.HasValue)
            return query;

        return query.Where(x => x.IsEnabled == isEnabled.Value);
    }

    public static IQueryable<TransportProfile> FilterByTransportType(
        this IQueryable<TransportProfile> query,
        TransportType? transportType)
    {
        if (!transportType.HasValue)
            return query;

        return query.Where(x => x.TransportType == transportType.Value);
    }

    public static IOrderedQueryable<TransportProfile> ApplySorting(
        this IQueryable<TransportProfile> query,
        TransportProfileSortBy sortBy,
        SortDirection direction)
    {
        return (sortBy, direction) switch
        {
            (TransportProfileSortBy.UpdatedAtUtc, SortDirection.Asc) => query
                .OrderBy(x => x.UpdatedAtUtc)
                .ThenBy(x => x.Id),

            (TransportProfileSortBy.UpdatedAtUtc, SortDirection.Desc) => query
                .OrderByDescending(x => x.UpdatedAtUtc)
                .ThenBy(x => x.Id),

            (TransportProfileSortBy.GlobalPriority, SortDirection.Desc) => query
                .OrderByDescending(x => x.GlobalPriority)
                .ThenBy(x => x.Id),

            _ => query
                .OrderBy(x => x.GlobalPriority)
                .ThenBy(x => x.Id)
        };
    }
}
