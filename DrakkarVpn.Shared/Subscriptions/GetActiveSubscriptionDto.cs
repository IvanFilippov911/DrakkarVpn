namespace DrakkarVpn.Shared.Subscriptions;

public sealed record GetActiveSubscriptionDto(Guid Id, DateTime StartAt, DateTime EndAt);