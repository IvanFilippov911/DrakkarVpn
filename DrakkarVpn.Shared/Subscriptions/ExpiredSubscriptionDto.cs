namespace DrakkarVpn.Shared.Subscriptions;

public sealed record ExpiredSubscriptionDto(Guid Id, Guid UserId, DateTime EndAt);