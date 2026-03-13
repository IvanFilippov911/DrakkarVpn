namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record DeviceRevokeRow(
    Guid UserId,
    string DeviceId
);