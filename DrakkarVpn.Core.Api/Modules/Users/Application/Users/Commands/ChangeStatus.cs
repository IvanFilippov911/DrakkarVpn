using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Commands;

public sealed record ChangeStatus(Guid UserId, UserStatus NewStatus) : IRequest<AppUserDto>;

public sealed class ChangeStatusHandler : IRequestHandler<ChangeStatus, AppUserDto>
{
    private readonly IAppUserRepository _repo;

    public ChangeStatusHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto> Handle(ChangeStatus req, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(req.UserId, ct)
                   ?? throw new InvalidOperationException("User not found");

        user.SetStatus(req.NewStatus);
        await _repo.SaveChangesAsync(ct);

        return new AppUserDto(user.Id, (long)user.TelegramId, user.CreatedAt, user.Status.ToString());
    }
}