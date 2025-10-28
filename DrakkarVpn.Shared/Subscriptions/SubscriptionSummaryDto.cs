namespace DrakkarVpn.Shared.Subscriptions;

public sealed record SubscriptionSummaryDto(
    Guid Id,
    bool IsActive,
    DateTime EndAtUtc,
    int MaxDevices
);