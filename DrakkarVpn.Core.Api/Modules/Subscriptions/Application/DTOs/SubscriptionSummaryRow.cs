namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record SubscriptionSummaryRow(
    Guid Id,
    Guid UserId,
    bool IsActive,
    DateTime EndAtUtc,
    int MaxDevices
);