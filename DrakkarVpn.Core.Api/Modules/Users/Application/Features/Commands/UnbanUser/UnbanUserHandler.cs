using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.UnbanUser;

public sealed class UnbanUserHandler : IRequestHandler<UnbanUserCommand, bool>
{
    private readonly IAppUserRepository _users;

    public UnbanUserHandler(IAppUserRepository users)
    {
        _users = users;
    }

    public async Task<bool> Handle(UnbanUserCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetForUpdateAsync(cmd.UserId, ct);
        if (user is null)
            return false;

        user.Unban();
        return true;
    }
}