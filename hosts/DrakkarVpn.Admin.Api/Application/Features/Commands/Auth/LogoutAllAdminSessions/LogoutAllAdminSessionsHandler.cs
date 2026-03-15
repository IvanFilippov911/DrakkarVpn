using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LogoutAllAdminSessions;

public sealed class LogoutAllAdminSessionsHandler
    : IRequestHandler<LogoutAllAdminSessionsCommand, Unit>
{
    private readonly IAdminAuthService _adminAuthService;

    public LogoutAllAdminSessionsHandler(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    public async Task<Unit> Handle(LogoutAllAdminSessionsCommand command, CancellationToken ct)
    {
        await _adminAuthService.LogoutAllAsync(ct);
        return Unit.Value;
    }
}
