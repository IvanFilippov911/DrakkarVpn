using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure; 
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;

public sealed class AppUserRepository : IAppUserRepository
{
    private readonly AppDbContext _db;
    public AppUserRepository(AppDbContext db) => _db = db;

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<AppUser?> GetByTelegramIdAsync(long tgId, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.TelegramId == tgId, ct);

    public async Task AddAsync(AppUser user, CancellationToken ct) =>
        await _db.Users.AddAsync(user, ct);

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct) =>
        await _db.Users.AsNoTracking().ToListAsync(ct);

    public async Task<(IReadOnlyList<UserIndexRowDto> Items, int Total)> SearchOnServerAsync(
        Guid serverId,
        int page, int pageSize,
        string? search,
        UserStatus? status,
        SubscriptionStatus? subscriptionStatus,
        UsersSortBy sortBy,
        CancellationToken ct)
    {
        var (skip, take) = ValidatePagination(page, pageSize);
        var nowUtc = DateTime.UtcNow;
        
        var usersBase = _db.Users
            .AsNoTracking()
            .WhereUserBelongsToServer(_db, serverId)
            .ApplySearch(search)
            .FilterByUserStatus(status);
        
        var joined = usersBase.WithMaxEndAndFilter(_db.Subscriptions.AsNoTracking(), subscriptionStatus, nowUtc);
        
        var sorted = joined.SortBy(sortBy);
        
        var items = await sorted
            .Skip(skip)
            .Take(take)
            .SelectProjection(_db, serverId)
            .ToListAsync(ct);

        var total = await joined.CountAsync(ct);

        return (items, total);
    }

    private static (int skip, int take) ValidatePagination(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return ((page - 1) * pageSize, pageSize);
    }
}