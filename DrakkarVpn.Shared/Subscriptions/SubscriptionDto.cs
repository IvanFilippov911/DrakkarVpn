namespace DrakkarVpn.Shared.Subscriptions;

public sealed record SubscriptionDto(
    Guid Id,
    Guid UserId,
    Guid TariffId,
    DateTime StartAt,
    DateTime EndAt,
    string Status
);