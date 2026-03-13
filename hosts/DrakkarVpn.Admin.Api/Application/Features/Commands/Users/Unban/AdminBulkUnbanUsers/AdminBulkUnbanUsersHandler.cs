using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Mappers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkUnbanUsers;

public sealed class AdminBulkUnbanUsersHandler
    : IRequestHandler<AdminBulkUnbanUsersCommand, BulkUsersOperationResponse>
{
    private readonly IUserModerationService _users;

    public AdminBulkUnbanUsersHandler(IUserModerationService users)
    {
        _users = users;
    }

    public async Task<BulkUsersOperationResponse> Handle(
        AdminBulkUnbanUsersCommand cmd,
        CancellationToken ct)
    {
        var res = await _users.UnbanUsersBulkAsync(cmd.UserIds, ct);
        return res.ToContract();
    }
}