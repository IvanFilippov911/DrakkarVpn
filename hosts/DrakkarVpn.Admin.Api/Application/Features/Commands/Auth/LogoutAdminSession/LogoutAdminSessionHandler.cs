using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LogoutAdminSession;

public sealed class LogoutAdminSessionHandler : IRequestHandler<LogoutAdminSessionCommand, Unit>
{
    private readonly IAdminAuthService _adminAuthService;

    public LogoutAdminSessionHandler(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    public async Task<Unit> Handle(LogoutAdminSessionCommand command, CancellationToken ct)
    {
        await _adminAuthService.LogoutAsync(new AdminLogoutSessionRequest(command.RefreshToken), ct);
        return Unit.Value;
    }
}
