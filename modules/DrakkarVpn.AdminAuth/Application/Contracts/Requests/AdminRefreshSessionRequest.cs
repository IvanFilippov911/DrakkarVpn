namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminRefreshSessionRequest(
    string RefreshToken,
    string? IpAddress,
    string? UserAgent);
