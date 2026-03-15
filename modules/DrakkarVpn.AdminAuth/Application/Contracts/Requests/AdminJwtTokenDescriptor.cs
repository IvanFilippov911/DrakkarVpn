namespace DrakkarVpn.AdminAuth.Application.Contracts;

public sealed record AdminJwtTokenDescriptor(
    Guid AdminId,
    string Email,
    IList<string> Roles);
