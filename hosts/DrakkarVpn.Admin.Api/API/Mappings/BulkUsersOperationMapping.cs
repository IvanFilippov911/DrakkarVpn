using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class BulkUsersOperationMapping
{
    public static BulkUsersOperationResponse ToResponse(this BulkUsersOperationResult r)
        => new(
            SucceededUserIds: r.SucceededUserIds,
            NotFoundUserIds:  r.NotFoundUserIds,
            FailedUserIds:    r.FailedUserIds,
            FailureDetails:   r.FailureDetails?.Select(ToResponse).ToList()
        );

    private static BulkUserFailureDetail ToResponse(BulkUserFailureDetail d)
        => new(
            UserId: d.UserId,
            Code: d.Code,
            Message: d.Message,
            Items: d.Items?.Select(i => new BulkUserFailureItem(
                PeerId: i.PeerId,
                ServerId: i.ServerId,
                Code: i.Code,
                Message: i.Message
            )).ToList()
        );
}