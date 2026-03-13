using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Extensions;

public static class CoreErrorEventQueryableExtensions
{
    public static IQueryable<CoreErrorEvent> ApplyFilters(
        this IQueryable<CoreErrorEvent> q,
        string? area,
        string? errorType,
        string? command,
        string? domainCode,
        string? userId,
        string? telegramId,
        string? search,
        DateTime? fromUtc,
        DateTime? toUtc)
    {
        if (!string.IsNullOrWhiteSpace(area))
            q = q.Where(x => x.Area == area);

        if (!string.IsNullOrWhiteSpace(errorType))
            q = q.Where(x => x.ErrorType == errorType);

        if (!string.IsNullOrWhiteSpace(command))
            q = q.Where(x => x.Command == command);

        if (!string.IsNullOrWhiteSpace(domainCode))
            q = q.Where(x => x.DomainCode == domainCode);

        if (!string.IsNullOrWhiteSpace(userId))
            q = q.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(telegramId))
            q = q.Where(x => x.TelegramId == telegramId);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.Message.Contains(search));

        if (fromUtc.HasValue)
            q = q.Where(x => x.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            q = q.Where(x => x.TimestampUtc <= toUtc.Value);

        return q;
    }
}