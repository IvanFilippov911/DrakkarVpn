namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record CoreErrorEventListItemDto(
    Guid Id,
    DateTime TimestampUtc,
    string Command,
    string Area,
    string ErrorType,
    string? DomainCode,
    string Message,
    string TraceId,
    string? UserId,
    string? TelegramId
);
