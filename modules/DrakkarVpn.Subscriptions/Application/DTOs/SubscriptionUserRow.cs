namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record SubscriptionUserRow(
    Guid SubscriptionId,
    Guid UserId
);