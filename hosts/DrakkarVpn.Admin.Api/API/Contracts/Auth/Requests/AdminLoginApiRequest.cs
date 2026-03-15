namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Auth.Requests;

public sealed record AdminLoginApiRequest(
    string Email,
    string Password);
