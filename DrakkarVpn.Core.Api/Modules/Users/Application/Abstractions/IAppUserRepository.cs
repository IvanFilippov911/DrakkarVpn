using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IAppUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AppUser?> GetForUpdateAsync(Guid id, CancellationToken ct);
    Task<AppUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);
    Task AddAsync(AppUser user, CancellationToken ct);
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct);
    Task<(IReadOnlyList<UserIndexRowDto> Items, int Total)> SearchOnServerAsync(
        Guid serverId,
        int page, int pageSize,
        string? search,
        UserStatus? status,
        SubscriptionStatus? subscriptionStatus,
        UsersSortBy sortBy,
        CancellationToken ct);
}