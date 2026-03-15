namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminLoginRequest(
    string Email,
    string Password,
    string? IpAddress,
    string? UserAgent);
