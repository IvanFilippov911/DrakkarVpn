

using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Mappers;

public static class BulkUsersOperationMapper
{
    public static BulkUsersOperationResponse ToContract(this BulkUsersOperationResult r)
        => new(
            SucceededUserIds: r.SucceededUserIds,
            NotFoundUserIds:  r.NotFoundUserIds,
            FailedUserIds:    r.FailedUserIds,
            FailureDetails:   r.FailureDetails
        );
}