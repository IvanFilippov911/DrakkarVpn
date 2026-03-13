namespace DrakkarVpn.Observability.Application.Commands;

public sealed record LogCoreErrorEventArgs(
    string Command,
    string Area,
    string ErrorType,
    string? DomainCode,
    string Message,
    string TraceId,
    string? UserId,
    string? TelegramId,
    string? PayloadJson
);