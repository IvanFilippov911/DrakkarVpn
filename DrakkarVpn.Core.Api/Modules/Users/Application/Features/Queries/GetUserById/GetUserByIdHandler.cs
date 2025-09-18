using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserById;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdRequest, AppUserDto?>
{
    private readonly IAppUserRepository _repo;

    public GetUserByIdHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto?> Handle(GetUserByIdRequest req, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(req.UserId, ct);
        return u is null 
            ? null 
            : new AppUserDto(u.Id, (long)u.TelegramId, u.CreatedAt, u.Status.ToString());
    }
}