using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.AdminAuth.Application.Contracts;
using DrakkarVpn.AdminAuth.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DrakkarVpn.AdminAuth.Infrastructure.Jwt;

public sealed class AdminJwtTokenService : IAdminJwtTokenService
{
    private readonly AdminJwtOptions _options;
    private readonly SigningCredentials _signingCredentials;

    public AdminJwtTokenService(IOptions<AdminJwtOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;

        var keyBytes = TryFromBase64(_options.SigningKey) ?? Encoding.UTF8.GetBytes(_options.SigningKey);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("AdminJwt:SigningKey must be at least 32 bytes (256-bit). Use Base64 string.");

        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);
    }

    public AdminAccessToken Issue(AdminJwtTokenDescriptor descriptor, TimeSpan? ttl = null)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentException.ThrowIfNullOrWhiteSpace(descriptor.Email);

        var jti = Guid.NewGuid().ToString("N");
        var issuedAtUtc = DateTime.UtcNow;
        var expiresAtUtc = issuedAtUtc.Add(ttl ?? TimeSpan.FromMinutes(_options.AccessTokenTtlMinutes));

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
            new(JwtRegisteredClaimNames.Email, descriptor.Email),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim(AdminClaimTypes.Permission, permission)));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: expiresAtUtc,
            signingCredentials: _signingCredentials);

        var value = new JwtSecurityTokenHandler().WriteToken(token);
        return new AdminAccessToken(value, expiresAtUtc, jti);
    }

    private static byte[]? TryFromBase64(string input)
    {
        try
        {
            return Convert.FromBase64String(input);
        }
        catch
        {
            return null;
        }
    }
}
