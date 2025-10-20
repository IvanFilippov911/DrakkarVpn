namespace DrakkarVpn.Shared.Subscriptions;

public sealed record GetActiveSubscriptionDto(Guid Id, int MaxDevices, DateTime StartAt, DateTime EndAt);