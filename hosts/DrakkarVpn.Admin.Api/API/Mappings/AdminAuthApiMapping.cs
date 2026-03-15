using DrakkarVpn.AdminAuth.Application.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Auth.Responses;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminAuthApiMapping
{
    public static AdminAuthApiResponse ToResponse(this AdminLoginResult result)
    {
        return new AdminAuthApiResponse(
            result.Tokens.AccessToken,
            result.Tokens.AccessTokenExpiresAtUtc,
            result.Tokens.AccessTokenId,
            result.Admin.ToResponse());
    }

    public static AdminAuthApiResponse ToResponse(this AdminRefreshSessionResult result)
    {
        return new AdminAuthApiResponse(
            result.Tokens.AccessToken,
            result.Tokens.AccessTokenExpiresAtUtc,
            result.Tokens.AccessTokenId,
            result.Admin.ToResponse());
    }

    public static AdminCurrentAdminApiResponse ToResponse(this AdminCurrentAdminProfile profile)
    {
        return new AdminCurrentAdminApiResponse(
            profile.AdminId,
            profile.Email,
            profile.Roles,
            profile.Permissions);
    }
}
