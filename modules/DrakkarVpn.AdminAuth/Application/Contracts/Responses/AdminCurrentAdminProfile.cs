namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminCurrentAdminProfile(
    Guid AdminId,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
