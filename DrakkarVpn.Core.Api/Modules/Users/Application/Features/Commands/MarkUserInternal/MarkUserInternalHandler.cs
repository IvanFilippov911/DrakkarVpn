using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.MarkUserInternal;

public sealed class MarkUserInternalHandler 
    : IRequestHandler<MarkUserInternalCommand, bool>
{
    private readonly IAppUserRepository _users;

    public MarkUserInternalHandler(IAppUserRepository users)
    {
        _users = users;
    }

    public async Task<bool> Handle(MarkUserInternalCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetForUpdateAsync(cmd.UserId, ct);
        if (user is null)
            return false;

        user.SetInternal(cmd.IsInternal);
        return true;
    }
}