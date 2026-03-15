namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminIssuedRefreshSession(
    Guid AdminUserId,
    string RefreshToken,
    DateTime ExpiresAtUtc);
