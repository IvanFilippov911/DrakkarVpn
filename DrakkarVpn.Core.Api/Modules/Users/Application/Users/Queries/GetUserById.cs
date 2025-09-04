using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Queries;

public sealed record GetUserById(Guid UserId) : IRequest<AppUserDto?>;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserById, AppUserDto?>
{
    private readonly IAppUserRepository _repo;

    public GetUserByIdHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto?> Handle(GetUserById req, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(req.UserId, ct);
        return u is null 
            ? null 
            : new AppUserDto(u.Id, (long)u.TelegramId, u.CreatedAt, u.Status.ToString());
    }
}