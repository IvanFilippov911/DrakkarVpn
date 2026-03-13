namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record BulkDevicesRevokeResult(
    IReadOnlyList<Guid> SucceededUserIds,
    IReadOnlyList<Guid> FailedUserIds,
    IReadOnlyList<BulkDeviceRevokeFailure> FailureDetails
);