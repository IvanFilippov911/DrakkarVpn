using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Extensions;

public static class CoreAlertQueryableExtensions
{
    public static IQueryable<CoreAlert> ApplyFilters(
        this IQueryable<CoreAlert> q,
        bool? isResolved,
        string? source,
        string? severity,
        DateTime? fromUtc,
        DateTime? toUtc)
    {
        if (isResolved.HasValue)
            q = q.Where(x => x.IsResolved == isResolved.Value);

        if (!string.IsNullOrWhiteSpace(source))
            q = q.Where(x => x.Source == source);

        if (!string.IsNullOrWhiteSpace(severity))
            q = q.Where(x => x.Severity == severity);

        if (fromUtc.HasValue)
            q = q.Where(x => x.CreatedAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            q = q.Where(x => x.CreatedAtUtc <= toUtc.Value);

        return q;
    }
}