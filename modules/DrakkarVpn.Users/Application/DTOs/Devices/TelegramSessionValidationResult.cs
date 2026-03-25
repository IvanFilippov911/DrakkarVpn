namespace DrakkarVpn.Users.Application.DTOs.Devices;

public sealed record TelegramSessionValidationResult(
    long TelegramId,
    DateTime AuthDateUtc,
    string ReplayKey);