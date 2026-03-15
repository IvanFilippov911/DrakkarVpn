using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface IAdminAuthenticationService
{
    Task<AdminJwtTokenDescriptor> AuthenticateAsync(
        string email,
        string password,
        CancellationToken ct);

    Task<AdminJwtTokenDescriptor> GetActiveAdminAsync(
        Guid adminId,
        CancellationToken ct);
}
