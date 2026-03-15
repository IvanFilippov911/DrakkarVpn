using System.Security.Cryptography;
using System.Text;
using DrakkarVpn.AdminAuth.Application.Abstractions.Repositories;
using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using DrakkarVpn.AdminAuth.Application.Exceptions;
using DrakkarVpn.AdminAuth.Application.Options;
using DrakkarVpn.AdminAuth.Infrastructure.EF;
using DrakkarVpn.AdminAuth.Infrastructure.EF.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DrakkarVpn.AdminAuth.Infrastructure.Auth;

public sealed class AdminRefreshSessionService : IAdminRefreshSessionService
{
    private readonly AdminAuthDbContext _db;
    private readonly IAdminRefreshTokenRepository _refreshTokens;
    private readonly AdminRefreshTokenOptions _options;

    public AdminRefreshSessionService(
        AdminAuthDbContext db,
        IAdminRefreshTokenRepository refreshTokens,
        IOptions<AdminRefreshTokenOptions> options)
    {
        _db = db;
        _refreshTokens = refreshTokens;
        _options = options.Value;
    }

    public async Task<AdminIssuedRefreshSession> CreateAsync(
        Guid adminId,
        string? ipAddress,
        string? userAgent,
        CancellationToken ct)
    {
        var issued = CreateRefreshTokenEntity(adminId, ipAddress, userAgent, DateTime.UtcNow);
        await _refreshTokens.AddAsync(issued.Token, ct);

        return new AdminIssuedRefreshSession(
            issued.Token.AdminUserId,
            issued.PlaintextToken,
            issued.Token.ExpiresAtUtc);
    }

    public async Task<AdminIssuedRefreshSession> RotateAsync(
        string refreshToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        var currentTokenHash = HashToken(refreshToken);
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var currentSession = await _refreshTokens.GetByTokenHashForUpdateAsync(currentTokenHash, ct);
        if (currentSession is null)
            throw new InvalidRefreshTokenException();

        var nowUtc = DateTime.UtcNow;
        if (currentSession.RevokedAtUtc is not null)
            throw new InvalidRefreshTokenException("Refresh token is revoked");

        if (currentSession.ExpiresAtUtc <= nowUtc)
            throw new InvalidRefreshTokenException("Refresh token is expired");

        var replacement = CreateRefreshTokenEntity(
            currentSession.AdminUserId,
            ipAddress,
            userAgent,
            nowUtc);

        currentSession.RevokedAtUtc = nowUtc;
        currentSession.ReplacedByTokenHash = replacement.Token.TokenHash;

        await _refreshTokens.AddAsync(replacement.Token, ct);
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new AdminIssuedRefreshSession(
            replacement.Token.AdminUserId,
            replacement.PlaintextToken,
            replacement.Token.ExpiresAtUtc);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        var tokenHash = HashToken(refreshToken);
        var currentSession = await _refreshTokens.GetByTokenHashAsync(tokenHash, ct);
        if (currentSession is null || currentSession.RevokedAtUtc is not null)
            return;

        currentSession.RevokedAtUtc = DateTime.UtcNow;
    }

    public async Task RevokeAllAsync(Guid adminId, CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;
        var sessions = await _refreshTokens.GetActiveByAdminUserIdAsync(adminId, nowUtc, ct);
        if (sessions.Count == 0)
            return;

        foreach (var session in sessions)
            session.RevokedAtUtc = nowUtc;
    }

    private IssuedRefreshSession CreateRefreshTokenEntity(
        Guid adminId,
        string? ipAddress,
        string? userAgent,
        DateTime nowUtc)
    {
        var plaintextToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        var tokenHash = HashToken(plaintextToken);

        var refreshToken = new AdminRefreshToken
        {
            AdminUserId = adminId,
            TokenHash = tokenHash,
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = nowUtc.AddDays(_options.LifetimeDays),
            CreatedByIp = Normalize(ipAddress, 128),
            UserAgent = Normalize(userAgent, 1024)
        };

        return new IssuedRefreshSession(plaintextToken, refreshToken);
    }

    private static string HashToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes);
    }

    private static string? Normalize(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength
            ? trimmed
            : trimmed[..maxLength];
    }

    private sealed record IssuedRefreshSession(
        string PlaintextToken,
        AdminRefreshToken Token);
}
