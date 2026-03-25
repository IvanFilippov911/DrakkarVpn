using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IUserRegistrationService
{
    Task<RegisterOrGetResultDto> RegisterOrGetAsync(
        long telegramId,
        DateTime nowUtc,
        CancellationToken ct);
}