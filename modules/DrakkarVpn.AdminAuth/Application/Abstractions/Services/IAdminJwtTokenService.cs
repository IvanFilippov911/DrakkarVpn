using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface IAdminJwtTokenService
{
    AdminAccessToken Issue(AdminJwtTokenDescriptor descriptor, TimeSpan? ttl = null);
}
