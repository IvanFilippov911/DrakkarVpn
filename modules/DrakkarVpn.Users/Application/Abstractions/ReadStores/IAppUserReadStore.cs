using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories.rowDTOs;
using DrakkarVpn.Shared.Subscriptions;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IAppUserReadStore
{
    Task<(IReadOnlyList<UserIndexRowDto> Items, int Total)> GetUsersListAsync(
        int page,
        int pageSize,
        string? search,
        UserStatus? status,
        SubscriptionStatus? subscriptionStatus,
        UsersSortBy sortBy,
        UserSortDirection userSortDirection,
        CancellationToken ct);
    
    Task<UserSummaryDetailDto?> GetSummaryAsync(Guid userId, CancellationToken ct);

    Task<IReadOnlyList<Guid>> GetExistingIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct);

    Task<List<Guid>> GetByModerationMarkerAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        ModerationMarkerCheck check,
        CancellationToken ct);
}