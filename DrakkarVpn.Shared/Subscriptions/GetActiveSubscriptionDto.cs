namespace DrakkarVpn.Shared.Subscriptions;

public sealed record GetActiveSubscriptionDto(Guid Id, Guid TariffId, DateTime StartAt, DateTime EndAt);