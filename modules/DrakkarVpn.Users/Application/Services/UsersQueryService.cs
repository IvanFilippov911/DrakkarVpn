using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Subscriptions;
using DrakkarVpn.Users.Application.Abstractions;

namespace DrakkarVpn.Users.Application.Features.Services;

public sealed class UsersQueryService : IUsersQueryService
{
    private readonly IAppUserRepository _users;
    private readonly IDeviceRepository _devices;
    private readonly IAppUserReadStore _store;

    public UsersQueryService(
        IAppUserRepository users,
        IDeviceRepository devices,
        IAppUserReadStore store)
    {
        _users = users;
        _devices = devices;
        _store = store;
    }

    public Task<AppUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct)
        => _users.GetByTelegramIdAsync(telegramId, ct);

    public Task<int> CountActiveDevicesByUserAsync(Guid userId, CancellationToken ct)
        => _devices.CountActiveByUserAsync(userId, ct);

    public async Task<PagedResponseDto<UserCardDto>> GetUsersList(
        int page,
        int pageSize,
        string? search,
        UserStatus? status,
        SubscriptionStatus? subscriptionStatus,
        UsersSortBy sortBy,
        UserSortDirection userSortDirection,
        CancellationToken ct)
    {
        var (rows, total) = await _store.GetUsersListAsync(
            page: page,
            pageSize: pageSize,
            search: search,
            status: status,
            subscriptionStatus: subscriptionStatus,
            sortBy: sortBy,
            userSortDirection: userSortDirection,
            ct: ct);

        if (rows.Count == 0)
            return PagedResponseDto<UserCardDto>.Empty(page, pageSize);

        var items = rows.Select(r =>
        {
            var user = new UserSummaryDto(
                Id:           r.UserId,
                Telegram:     r.TelegramId,
                CreatedAtUtc: r.CreatedAtUtc,
                Status:       r.Status,
                IsOnline:     r.IsOnline,
                DeviceCount:  r.DeviceCount,
                LastSeenUtc:  r.LastSeenUtc
            );

            var subscription = new SubscriptionSummaryDto(
                IsActive:   r.SubscriptionIsActive,
                EndAtUtc:   r.SubscriptionEndUtc ?? DateTime.UnixEpoch,
                MaxDevices: r.SubscriptionMaxDevices
            );

            return new UserCardDto(
                User: user,
                Subscription: subscription,
                TrafficLast24hBytes: r.TrafficLast24hBytes
            );
        }).ToList();

        return PagedResponseDto<UserCardDto>.From(items, page, pageSize, total);
    }
}