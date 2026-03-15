using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Infrastructure.Auth;

public sealed class AdminProfileFactory : IAdminProfileFactory
{
    public AdminCurrentAdminProfile Create(
        Guid adminId,
        string email,
        IEnumerable<string> roles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(roles);

        var normalizedRoles = roles
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var permissions = normalizedRoles
            .SelectMany(AdminRolePermissions.GetPermissions)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return new AdminCurrentAdminProfile(
            adminId,
            email,
            normalizedRoles,
            permissions);
    }
}
