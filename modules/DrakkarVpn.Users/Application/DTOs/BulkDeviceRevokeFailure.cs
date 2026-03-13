namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record BulkDeviceRevokeFailure(
    Guid UserId,
    string DeviceId,
    string Reason
);