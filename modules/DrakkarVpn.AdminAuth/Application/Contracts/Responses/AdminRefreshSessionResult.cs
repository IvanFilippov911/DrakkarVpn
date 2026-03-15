namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminRefreshSessionResult(
    AdminSessionTokens Tokens,
    AdminCurrentAdminProfile Admin);
