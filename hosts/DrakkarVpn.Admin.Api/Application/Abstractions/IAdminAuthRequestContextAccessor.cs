namespace DrakkarVpn.Core.Api.Modules.Admin.API.Auth;

public interface IAdminAuthRequestContextAccessor
{
    string? GetIpAddress();
    string? GetUserAgent();
}
