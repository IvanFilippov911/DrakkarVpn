using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface ICurrentAdminAccessor
{
    AdminCurrentAdminProfile? GetCurrent();
    AdminCurrentAdminProfile GetRequiredCurrent();
}
