namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record BulkCancelSubscriptionsResponse(
    IReadOnlyList<Guid> Succeeded,
    IReadOnlyList<Guid> NotFound,
    IReadOnlyList<Guid> Failed,
    IReadOnlyList<SubscriptionUserRow> SucceededSubscriptionUserRows,
    IReadOnlyList<Guid> SucceededUserIds,
    IReadOnlyList<BulkSubscriptionFailureDetail>? FailureDetails = null
)
{
    public static BulkCancelSubscriptionsResponse Empty(IReadOnlyList<Guid> notFound) => new(
        Succeeded: Array.Empty<Guid>(),
        NotFound: notFound,
        Failed: Array.Empty<Guid>(),
        SucceededSubscriptionUserRows: Array.Empty<SubscriptionUserRow>(),
        SucceededUserIds: Array.Empty<Guid>()
    );
}