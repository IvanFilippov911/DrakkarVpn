namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record AppUserDto(
    Guid Id,
    long TelegramId,
    DateTime CreatedAt,
    string Status
);