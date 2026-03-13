using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminMarkUserInternal;

public sealed class AdminMarkUserInternalHandler
    : IRequestHandler<AdminMarkUserInternalCommand, bool>
{
    private readonly IUserModerationService _users;

    public AdminMarkUserInternalHandler(IUserModerationService users) => _users = users;

    public async Task<bool> Handle(AdminMarkUserInternalCommand cmd, CancellationToken ct)
    {
        var res = await _users.MarkInternalBulkAsync(new[] { cmd.UserId }, cmd.IsInternal, ct);
        return res.SucceededUserIds.Count == 1;
    }
}