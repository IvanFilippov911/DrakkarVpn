namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Auth.Responses;

public sealed record AdminCurrentAdminApiResponse(
    Guid AdminId,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
