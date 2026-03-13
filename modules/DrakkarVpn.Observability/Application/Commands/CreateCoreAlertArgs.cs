namespace DrakkarVpn.Observability.Application.Commands;

public sealed record CreateCoreAlertArgs(
    string Source,
    string Code,
    string Severity,
    string Title,
    string Message,
    Guid? ServerId,
    Guid? UserId,
    string? Region,
    string? DetailsJson
);