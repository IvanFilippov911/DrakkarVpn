namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record BulkUserGrantFailureDetail(
    Guid UserId,
    string Code,
    string Message
);