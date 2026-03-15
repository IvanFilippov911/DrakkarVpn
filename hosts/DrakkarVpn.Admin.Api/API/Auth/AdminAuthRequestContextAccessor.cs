using Microsoft.AspNetCore.Http;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Auth;

public sealed class AdminAuthRequestContextAccessor : IAdminAuthRequestContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminAuthRequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }

    public string? GetUserAgent()
    {
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
        return string.IsNullOrWhiteSpace(userAgent) ? null : userAgent;
    }
}
