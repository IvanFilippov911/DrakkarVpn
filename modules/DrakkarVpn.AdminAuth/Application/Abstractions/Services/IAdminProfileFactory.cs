using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface IAdminProfileFactory
{
    AdminCurrentAdminProfile Create(
        Guid adminId,
        string email,
        IEnumerable<string> roles);
}
