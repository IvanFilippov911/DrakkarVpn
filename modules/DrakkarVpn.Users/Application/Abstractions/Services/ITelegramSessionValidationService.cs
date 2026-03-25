using DrakkarVpn.Users.Application.DTOs.Devices;

namespace DrakkarVpn.Users.Application.Abstractions.Services;

public interface ITelegramSessionValidationService
{
    Task<TelegramSessionValidationResult> ValidateAsync(string initData, CancellationToken ct);
}