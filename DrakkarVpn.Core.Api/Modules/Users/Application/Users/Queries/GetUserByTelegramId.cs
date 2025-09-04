using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Queries;

public sealed record GetUserByTelegramId(long TelegramId) : IRequest<AppUserDto?>;

public sealed class GetUserByTelegramIdHandler : IRequestHandler<GetUserByTelegramId, AppUserDto?>
{
    private readonly IAppUserRepository _repo;

    public GetUserByTelegramIdHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto?> Handle(GetUserByTelegramId req, CancellationToken ct)
    {
        var u = await _repo.GetByTelegramIdAsync((TelegramId)req.TelegramId, ct);
        return u is null
            ? null
            : new AppUserDto(u.Id, (long)u.TelegramId, u.CreatedAt, u.Status.ToString());
    }
}