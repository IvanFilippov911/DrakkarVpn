namespace DrakkarVpn.Observability.Application.DTOs;

public sealed record GetCoreErrorEventsDto(
    int Page,
    int PageSize,
    string? Area,
    string? ErrorType,
    string? Command,
    string? DomainCode,
    string? UserId,
    string? TelegramId,
    string? Search,
    DateTime? FromUtc,
    DateTime? ToUtc
);