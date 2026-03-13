namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record BulkSubscriptionFailureItem(
    Guid? UserId,
    Guid? PeerId,
    Guid? ServerId,
    string Code,
    string? Message = null
);