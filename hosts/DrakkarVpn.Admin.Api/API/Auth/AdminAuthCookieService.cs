using DrakkarVpn.AdminAuth.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Auth;

public sealed class AdminAuthCookieService : IAdminAuthCookieService
{
    private const string RefreshTokenCookieName = "drakkar_admin_refresh_token";
    private const string RefreshTokenCookiePath = "/api/admin/auth";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminAuthCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetRequiredRefreshToken()
    {
        if (!TryGetRefreshToken(out var refreshToken))
            throw new MissingRefreshTokenException();

        return refreshToken;
    }

    public bool TryGetRefreshToken(out string refreshToken)
    {
        var request = GetRequiredHttpContext().Request;

        if (request.Cookies.TryGetValue(RefreshTokenCookieName, out var value) &&
            !string.IsNullOrWhiteSpace(value))
        {
            refreshToken = value;
            return true;
        }

        refreshToken = string.Empty;
        return false;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiresAtUtc)
    {
        GetRequiredHttpContext().Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            CreateCookieOptions(expiresAtUtc));
    }

    public void DeleteRefreshToken()
    {
        GetRequiredHttpContext().Response.Cookies.Delete(
            RefreshTokenCookieName,
            CreateCookieOptions());
    }

    private static CookieOptions CreateCookieOptions(DateTime? expiresAtUtc = null)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = RefreshTokenCookiePath,
            IsEssential = true,
            Expires = expiresAtUtc is null ? null : new DateTimeOffset(expiresAtUtc.Value)
        };
    }

    private HttpContext GetRequiredHttpContext()
    {
        return _httpContextAccessor.HttpContext
               ?? throw new InvalidOperationException("HttpContext is unavailable");
    }
}
