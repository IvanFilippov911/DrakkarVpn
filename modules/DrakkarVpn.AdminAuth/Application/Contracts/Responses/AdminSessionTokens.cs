namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminSessionTokens(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string AccessTokenId,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
