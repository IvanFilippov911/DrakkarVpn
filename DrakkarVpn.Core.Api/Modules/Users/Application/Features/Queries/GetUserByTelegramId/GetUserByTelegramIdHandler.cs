using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;

public sealed class GetUserByTelegramIdHandler : IRequestHandler<GetUserByTelegramIdRequest, AppUserDto?>
{
    private readonly IAppUserRepository _repo;

    public GetUserByTelegramIdHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto?> Handle(GetUserByTelegramIdRequest req, CancellationToken ct)
    {
        var u = await _repo.GetByTelegramIdAsync(req.TelegramId, ct);
        return u is null
            ? null
            : new AppUserDto(u.Id, (long)u.TelegramId, u.CreatedAt, u.Status.ToString());
    }
}