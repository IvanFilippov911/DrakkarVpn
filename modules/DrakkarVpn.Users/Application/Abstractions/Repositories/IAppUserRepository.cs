using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IAppUserRepository
{
    Task<AppUser?> GetForUpdateAsync(Guid id, CancellationToken ct);
    Task AddAsync(AppUser user, CancellationToken ct);
    Task<AppUser?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);

    Task<int> BanManyAsync(
        IReadOnlyCollection<Guid> existingUserIds,
        string? reason,
        DateTime nowUtc,
        CancellationToken ct);

    Task<int> UnbanManyAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime nowUtc,
        CancellationToken ct);
    
    Task SetInternalManyAsync(
        IReadOnlyCollection<Guid> userIds,
        bool isInternal,
        DateTime markerUtc,
        CancellationToken ct);

}