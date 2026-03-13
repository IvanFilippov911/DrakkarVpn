using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Extensions;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories.rowDTOs;
using DrakkarVpn.Shared.Subscriptions;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs.Admin;
using DrakkarVpn.Users.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Users.Infrastructure.EF.Repositories.ReadRepositories;

public sealed class AppUserReadStore : IAppUserReadStore
{
    private readonly UsersDbContext _db;
    public AppUserReadStore(UsersDbContext db) => _db = db;
    
    public async Task<(IReadOnlyList<UserIndexRowDto> Items, int Total)> GetUsersListAsync(
        int page,
        int pageSize,
        string? search,
        UserStatus? status,
        SubscriptionStatus? subscriptionStatus,
        UsersSortBy sortBy,
        UserSortDirection userSortDirection,
        CancellationToken ct)
    {
        var (skip, take) = ValidatePagination(page, pageSize);
        var nowUtc = DateTime.UtcNow;

        var baseUsers = _db.Users
            .AsNoTracking()
            .ApplySearch(search)
            .FilterByUserStatus(status);

        var query = baseUsers
            .WithRealtimeStats(_db.UserRealtimeStats.AsNoTracking())
            .FilterBySubscriptionStatus(subscriptionStatus, nowUtc);

        var total = await query.CountAsync(ct);

        var sorted = query.ApplySorting(sortBy, userSortDirection);

        var items = await sorted
            .Skip(skip)
            .Take(take)
            .ProjectToUserIndex()
            .ToListAsync(ct);

        return (items, total);
    }

    private static (int skip, int take) ValidatePagination(int page, int pageSize)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return ((page - 1) * pageSize, pageSize);
    }
    
    public async Task<UserSummaryDetailDto?> GetSummaryAsync(
        Guid userId,
        CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserSummaryDetailDto(
                u.Id,
                u.TelegramId,
                u.Username,
                u.CreatedAt,
                u.Status,
                u.IsInternal,
                u.BanReason,
                u.BannedAtUtc
            ))
            .SingleOrDefaultAsync(ct);
    }
    
    public async Task<IReadOnlyList<Guid>> GetExistingIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(ct);
    }
    
    public Task<List<Guid>> GetByModerationMarkerAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        ModerationMarkerCheck check,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        var q = _db.Users.AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .Where(u => u.ModerationUpdatedAtUtc == markerUtc);

        q = check switch
        {
            ModerationMarkerCheck.Unbanned => q.Where(u =>
                u.Status == UserStatus.Active &&
                u.BannedAtUtc == null &&
                u.BanReason == null),

            ModerationMarkerCheck.Banned => q.Where(u =>
                u.Status == UserStatus.Banned &&
                u.BannedAtUtc != null),

            ModerationMarkerCheck.InternalOn => q.Where(u => u.IsInternal),
            ModerationMarkerCheck.InternalOff => q.Where(u => !u.IsInternal),

            _ => q
        };

        return q.Select(u => u.Id).ToListAsync(ct);
    }
    
    
}