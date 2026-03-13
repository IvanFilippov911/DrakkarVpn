using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;

public interface ICoreErrorEventRepository
{
    Task AddAsync(CoreErrorEvent evt, CancellationToken ct);

    Task<IReadOnlyList<CoreErrorEvent>> GetPagedAsync(
        int page,
        int pageSize,
        string? area,
        string? errorType,
        string? command,
        string? domainCode,
        string? userId,
        string? telegramId,
        string? search,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    Task<int> CountAsync(
        string? area,
        string? errorType,
        string? command,
        string? domainCode,
        string? userId,
        string? telegramId,
        string? search,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}