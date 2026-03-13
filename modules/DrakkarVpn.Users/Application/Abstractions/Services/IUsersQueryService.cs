using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IUsersQueryService
{
    Task<AppUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);

    Task<int> CountActiveDevicesByUserAsync(Guid userId, CancellationToken ct);

    Task<PagedResponseDto<UserCardDto>> GetUsersList(
        int page,
        int pageSize,
        string? search,
        UserStatus? status,
        SubscriptionStatus? subscriptionStatus,
        UsersSortBy sortBy,
        UserSortDirection userSortDirection,
        CancellationToken ct);
}