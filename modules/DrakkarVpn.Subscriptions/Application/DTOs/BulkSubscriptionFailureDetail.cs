namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record BulkSubscriptionFailureDetail(
    Guid SubscriptionId,
    string Code,
    string Message,
    IReadOnlyList<BulkSubscriptionFailureItem>? Items = null
);