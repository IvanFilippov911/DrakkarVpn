namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Request;

public sealed record BulkCancelSubscriptionsRequest(
    IReadOnlyCollection<Guid> SubscriptionIds
);