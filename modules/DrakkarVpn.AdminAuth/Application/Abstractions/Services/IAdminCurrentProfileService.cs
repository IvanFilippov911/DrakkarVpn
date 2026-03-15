using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface IAdminCurrentProfileService
{
    Task<AdminCurrentAdminProfile> GetCurrentAsync(CancellationToken ct);
}
