namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Auth.Responses;

public sealed record AdminAuthApiResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string AccessTokenId,
    AdminCurrentAdminApiResponse Admin);
