namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record CoreAlertListItemDto(
    Guid Id,
    DateTime CreatedAtUtc,
    DateTime? ResolvedAtUtc,
    bool IsResolved,
    string Source,
    string Code,
    string Severity,
    string Title,
    string Message,
    Guid? ServerId,
    Guid? UserId,
    string? Region
);