using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Users.Application.Features.Services;

public sealed class UserRegistrationService : IUserRegistrationService
{
    private readonly IAppUserRepository _repo;

    public UserRegistrationService(IAppUserRepository repo) => _repo = repo;

    public async Task<RegisterOrGetResultDto> RegisterOrGetAsync(
        long telegramId,
        string? telegramUsername,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (telegramId <= 0)
            throw new ArgumentOutOfRangeException(nameof(telegramId));

        var existing = await _repo.GetByTelegramIdAsync(telegramId, ct);
        if (existing is not null)
        {
            existing.SetTelegramUsername(telegramUsername);
            return new RegisterOrGetResultDto(IsNew: false, UserId: existing.Id);
        }

        var created = AppUser.CreateNew(telegramId, nowUtc, telegramUsername);
        await _repo.AddAsync(created, ct);

        return new RegisterOrGetResultDto(IsNew: true, UserId: created.Id);
    }
}