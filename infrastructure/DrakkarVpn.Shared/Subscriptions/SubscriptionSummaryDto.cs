namespace DrakkarVpn.Shared.Subscriptions;

public sealed record SubscriptionSummaryDto(
    bool IsActive,
    DateTime EndAtUtc,
    int MaxDevices
);