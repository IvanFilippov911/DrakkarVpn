using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.RefreshAdminSession;

public sealed class RefreshAdminSessionHandler
    : IRequestHandler<RefreshAdminSessionCommand, AdminRefreshSessionResult>
{
    private readonly IAdminAuthService _adminAuthService;

    public RefreshAdminSessionHandler(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    public Task<AdminRefreshSessionResult> Handle(
        RefreshAdminSessionCommand command,
        CancellationToken ct)
    {
        return _adminAuthService.RefreshAsync(
            new AdminRefreshSessionRequest(
                command.RefreshToken,
                command.IpAddress,
                command.UserAgent),
            ct);
    }
}
