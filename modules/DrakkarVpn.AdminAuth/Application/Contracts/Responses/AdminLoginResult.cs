namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminLoginResult(
    AdminSessionTokens Tokens,
    AdminCurrentAdminProfile Admin);
