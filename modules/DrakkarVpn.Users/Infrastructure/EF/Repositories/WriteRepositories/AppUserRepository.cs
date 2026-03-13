using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;

public sealed class AppUserRepository : IAppUserRepository
{
    private readonly UsersDbContext _db;
    public AppUserRepository(UsersDbContext db) => _db = db;
    
    public Task<AppUser?> GetForUpdateAsync(Guid id, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    
    public async Task AddAsync(AppUser user, CancellationToken ct) =>
        await _db.Users.AddAsync(user, ct);
    
    public Task<AppUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId, ct);
    
    public async Task<int> BanManyAsync(
        IReadOnlyCollection<Guid> existingUserIds,
        string? reason,
        DateTime nowUtc,
        CancellationToken ct)
    {
        
        var normalizedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        
        var affected = await _db.Users
            .Where(u => existingUserIds.Contains(u.Id))
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Status, UserStatus.Banned)
                    .SetProperty(u => u.BanReason, normalizedReason)
                    .SetProperty(u => u.BannedAtUtc, nowUtc),
                ct);

        return affected;
    }
    
    public Task<int> UnbanManyAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        return _db.Users
            .Where(u => ids.Contains(u.Id))
            .Where(u => u.Status == UserStatus.Banned)
            .ExecuteUpdateAsync(set => set
                    .SetProperty(u => u.Status, UserStatus.Active)
                    .SetProperty(u => u.BanReason, (string?)null)
                    .SetProperty(u => u.BannedAtUtc, (DateTime?)null)
                    .SetProperty(u => u.ModerationUpdatedAtUtc, nowUtc),
                ct);
    }
    
    
    public Task SetInternalManyAsync(
        IReadOnlyCollection<Guid> userIds,
        bool isInternal,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        return _db.Users
            .Where(u => ids.Contains(u.Id))
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.IsInternal, isInternal)
                    .SetProperty(u => u.ModerationUpdatedAtUtc, markerUtc),
                ct);
    }
    
    
}