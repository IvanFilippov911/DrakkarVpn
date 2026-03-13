namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;

public sealed record AdminUserAlertDto(
    Guid     AlertId,
    DateTime CreatedAtUtc,
    bool     IsResolved,
    string   Severity,
    string   Title,
    string   Message
);