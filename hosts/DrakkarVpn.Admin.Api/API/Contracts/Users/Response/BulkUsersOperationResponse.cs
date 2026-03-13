using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;

public sealed record BulkUsersOperationResponse(
    IReadOnlyList<Guid> SucceededUserIds,
    IReadOnlyList<Guid> NotFoundUserIds,
    IReadOnlyList<Guid> FailedUserIds,
    IReadOnlyList<BulkUserFailureDetail>? FailureDetails = null
);