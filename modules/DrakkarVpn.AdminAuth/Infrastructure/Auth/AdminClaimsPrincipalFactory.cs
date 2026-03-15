using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Infrastructure.Auth;

public sealed class AdminClaimsPrincipalFactory
{
    public ClaimsPrincipal Create(
        AdminJwtTokenDescriptor descriptor,
        string authenticationType,
        string? tokenId)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationType);
        ArgumentException.ThrowIfNullOrWhiteSpace(descriptor.Email);

        var roles = descriptor.Roles
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var permissions = roles
            .SelectMany(AdminRolePermissions.GetPermissions)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, descriptor.AdminId.ToString()),
            new(ClaimTypes.NameIdentifier, descriptor.AdminId.ToString()),
            new(JwtRegisteredClaimNames.Email, descriptor.Email),
            new(ClaimTypes.Email, descriptor.Email)
        };

        if (!string.IsNullOrWhiteSpace(tokenId))
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, tokenId));

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim(AdminClaimTypes.Permission, permission)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
    }
}
