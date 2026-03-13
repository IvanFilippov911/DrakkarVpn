using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using BulkCancelSubscriptionsResponse = DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs.BulkCancelSubscriptionsResponse;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Mappers.Subscriptions;

public static class BulkCancelSubscriptionsMapping
{
    public static API.Contracts.Subscriptions.Response.BulkCancelSubscriptionsResponse ToContract(
        this BulkCancelSubscriptionsResponse dto,
        IReadOnlyList<BulkSubscriptionFailureDetail>? failureDetails = null)
        => new(
            Succeeded: dto.Succeeded,
            Failed: dto.Failed,
            FailureDetails: failureDetails ?? dto.FailureDetails
        );
}