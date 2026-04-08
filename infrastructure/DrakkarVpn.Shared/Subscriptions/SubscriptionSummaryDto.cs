namespace DrakkarVpn.Shared.Subscriptions;

public sealed record SubscriptionSummaryDto(
    DateTime EndAtUtc,
    int MaxDevices,
    SubscriptionStatus? LastSubscriptionStatus
);