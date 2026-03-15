using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface IAdminRefreshSessionService
{
    Task<AdminIssuedRefreshSession> CreateAsync(
        Guid adminId,
        string? ipAddress,
        string? userAgent,
        CancellationToken ct);

    Task<AdminIssuedRefreshSession> RotateAsync(
        string refreshToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken ct);

    Task RevokeAsync(string refreshToken, CancellationToken ct);

    Task RevokeAllAsync(Guid adminId, CancellationToken ct);
}
