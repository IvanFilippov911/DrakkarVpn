namespace DrakkarVpn.Observability.Application.DTOs.Alerts;

public sealed record UserAlertDto(
    Guid Id,
    DateTime CreatedAtUtc,
    bool IsResolved,
    string Severity,
    string Title,
    string Message
);