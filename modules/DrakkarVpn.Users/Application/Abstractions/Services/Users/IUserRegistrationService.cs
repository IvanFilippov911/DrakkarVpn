using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IUserRegistrationService
{
    Task<RegisterOrGetResultDto> RegisterOrGetAsync(
        long telegramId,
        string? telegramUsername,
        DateTime nowUtc,
        CancellationToken ct);
}