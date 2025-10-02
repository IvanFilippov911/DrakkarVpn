namespace DrakkarVpn.Shared.Subscriptions;

public sealed record SubscriptionDto(
    Guid Id,
    Guid UserId,
    DateTime StartAt,
    DateTime EndAt,
    string Status
);