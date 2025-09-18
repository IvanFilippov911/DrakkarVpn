using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeStatus;

public sealed class ChangeStatusHandler : IRequestHandler<ChangeStatusRequest, AppUserDto>
{
    private readonly IAppUserRepository _repo;

    public ChangeStatusHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<AppUserDto> Handle(ChangeStatusRequest req, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(req.UserId, ct)
                   ?? throw new InvalidOperationException("User not found");

        user.SetStatus(req.NewStatus);

        return new AppUserDto(user.Id, (long)user.TelegramId, user.CreatedAt, user.Status.ToString());
    }
}