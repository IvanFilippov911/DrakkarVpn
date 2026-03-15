using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using DrakkarVpn.AdminAuth.Application.Authorization;
using Microsoft.AspNetCore.Http;

namespace DrakkarVpn.AdminAuth.Infrastructure.Auth;

public sealed class CurrentAdminAccessor : ICurrentAdminAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentAdminAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public AdminCurrentAdminProfile? GetCurrent()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var principal = httpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return null;

        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                    ?? principal.FindFirst(ClaimTypes.Email)?.Value;

        if (!Guid.TryParse(sub, out var adminId))
            return null;

        if (string.IsNullOrWhiteSpace(email))
            return null;

        var roles = principal.FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var permissions = principal.FindAll(AdminClaimTypes.Permission)
            .Select(x => x.Value)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return new AdminCurrentAdminProfile(
            adminId,
            email,
            roles,
            permissions);
    }

    public AdminCurrentAdminProfile GetRequiredCurrent()
    {
        return GetCurrent() ?? throw new InvalidOperationException("Current admin context is unavailable");
    }
}
