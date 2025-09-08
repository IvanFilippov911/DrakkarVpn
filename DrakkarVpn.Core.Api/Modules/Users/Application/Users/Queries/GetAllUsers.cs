using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Queries;

public sealed record GetAllUsers : IRequest<IReadOnlyList<AppUserDto>>;

public sealed class GetAllUsersHandler 
    : IRequestHandler<GetAllUsers, IReadOnlyList<AppUserDto>>
{
    private readonly IAppUserRepository _repo;

    public GetAllUsersHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<AppUserDto>> Handle(GetAllUsers req, CancellationToken ct)
    {
        var users = await _repo.GetAllAsync(ct);
        return users
            .Select(u => new AppUserDto(u.Id, (long)u.TelegramId, u.CreatedAt, u.Status.ToString()))
            .ToList();
    }
}