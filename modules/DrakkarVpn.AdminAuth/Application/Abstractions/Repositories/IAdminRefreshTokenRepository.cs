using DrakkarVpn.AdminAuth.Infrastructure.EF.Entities;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Repositories;

public interface IAdminRefreshTokenRepository
{
    Task AddAsync(AdminRefreshToken refreshToken, CancellationToken ct = default);

    Task<AdminRefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<AdminRefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken ct = default);

    Task<AdminRefreshToken?> GetByTokenHashForUpdateAsync(
        string tokenHash,
        CancellationToken ct = default);

    Task<IReadOnlyList<AdminRefreshToken>> GetActiveByAdminUserIdAsync(
        Guid adminUserId,
        DateTime nowUtc,
        CancellationToken ct = default);
}
