using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Commands;

public sealed record RegisterOrGetByTelegram(long TelegramId) : IRequest<AppUserDto>;

public sealed class RegisterOrGetByTelegramHandler 
    : IRequestHandler<RegisterOrGetByTelegram, AppUserDto>
{
    private readonly IAppUserRepository _repo;

    public RegisterOrGetByTelegramHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto> Handle(RegisterOrGetByTelegram req, CancellationToken ct)
    {
        var tgId = (TelegramId)req.TelegramId;

        var existing = await _repo.GetByTelegramIdAsync(tgId, ct);
        if (existing is not null)
            return ToDto(existing);

        var created = AppUser.CreateNew(tgId, DateTime.UtcNow);
        await _repo.AddAsync(created, ct);
        await _repo.SaveChangesAsync(ct);

        return ToDto(created);
    }

    private static AppUserDto ToDto(AppUser u) =>
        new(u.Id, (long)u.TelegramId, u.CreatedAt, u.Status.ToString());
}