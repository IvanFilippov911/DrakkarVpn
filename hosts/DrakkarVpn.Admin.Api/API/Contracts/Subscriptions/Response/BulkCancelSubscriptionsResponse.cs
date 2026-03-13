using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;

public sealed record BulkCancelSubscriptionsResponse(
    IReadOnlyList<Guid> Succeeded,
    IReadOnlyList<Guid> Failed,
    IReadOnlyList<BulkSubscriptionFailureDetail>? FailureDetails = null
);