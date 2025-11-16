using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokeAllUserPeers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.BanUser;

public sealed class BanUserHandler : IRequestHandler<BanUserCommand, bool>
{
    private readonly IAppUserRepository _users;
    private readonly IMediator _mediator;

    public BanUserHandler(IAppUserRepository users, IMediator mediator)
    {
        _users    = users;
        _mediator = mediator;
    }

    public async Task<bool> Handle(BanUserCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetForUpdateAsync(cmd.UserId, ct);
        if (user is null)
            return false;

        user.Ban(cmd.Reason, DateTime.UtcNow);
        await _mediator.Send(new RevokeAllUserPeersCommand(cmd.UserId), ct);

        return true;
    }
}