namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminAccessToken(
    string Value,
    DateTime ExpiresAtUtc,
    string Jti);
