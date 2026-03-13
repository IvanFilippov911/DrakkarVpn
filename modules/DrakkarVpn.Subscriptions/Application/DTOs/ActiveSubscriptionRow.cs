namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record ActiveSubscriptionRow(
    Guid SubscriptionId,
    Guid UserId,
    DateTime EndAt
);