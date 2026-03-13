namespace DrakkarVpn.Users.Application.DTOs;

public sealed record AppUserDto(
    Guid Id,
    long TelegramId,
    DateTime CreatedAt,
    string Status
);