using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;

namespace DrakkarVpn.Users.Application.DTOs.Admin;

public sealed record BulkUsersOperationResult(
    IReadOnlyList<Guid> SucceededUserIds,
    IReadOnlyList<Guid> NotFoundUserIds,
    IReadOnlyList<Guid> FailedUserIds,
    IReadOnlyList<BulkUserFailureDetail>? FailureDetails = null
);