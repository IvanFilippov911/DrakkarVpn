using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminUnbanUser;

public sealed class AdminUnbanUserHandler
    : IRequestHandler<AdminUnbanUserCommand>
{
    private readonly IUserModerationService _users;

    public AdminUnbanUserHandler(IUserModerationService users)
        => _users = users;

    public async Task Handle(AdminUnbanUserCommand cmd, CancellationToken ct)
    {
        var res = await _users.UnbanUsersBulkAsync(
            new[] { cmd.UserId },
            ct);
        
        if (res.SucceededUserIds.Count == 0)
            throw new InvalidOperationException($"Failed to unban user {cmd.UserId}");
    }
}