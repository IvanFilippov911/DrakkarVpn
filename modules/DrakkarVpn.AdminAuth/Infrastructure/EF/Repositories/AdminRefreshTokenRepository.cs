using DrakkarVpn.AdminAuth.Application.Abstractions.Repositories;
using DrakkarVpn.AdminAuth.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF.Repositories;

public sealed class AdminRefreshTokenRepository : IAdminRefreshTokenRepository
{
    private readonly AdminAuthDbContext _db;

    public AdminRefreshTokenRepository(AdminAuthDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AdminRefreshToken refreshToken, CancellationToken ct = default)
    {
        await _db.RefreshTokens.AddAsync(refreshToken, ct);
    }

    public Task<AdminRefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<AdminRefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken ct = default)
    {
        return _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
    }

    public Task<AdminRefreshToken?> GetByTokenHashForUpdateAsync(
        string tokenHash,
        CancellationToken ct = default)
    {
        return _db.RefreshTokens
            .FromSqlInterpolated($"""
                                  SELECT *
                                  FROM admin_auth.admin_refresh_tokens
                                  WHERE token_hash = {tokenHash}
                                  FOR UPDATE
                                  """)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<AdminRefreshToken>> GetActiveByAdminUserIdAsync(
        Guid adminUserId,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        return await _db.RefreshTokens
            .Where(x => x.AdminUserId == adminUserId)
            .Where(x => x.RevokedAtUtc == null)
            .Where(x => x.ExpiresAtUtc > nowUtc)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }
}
