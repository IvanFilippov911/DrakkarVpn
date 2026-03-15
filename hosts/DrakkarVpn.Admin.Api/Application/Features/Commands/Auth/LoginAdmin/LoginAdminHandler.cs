using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LoginAdmin;

public sealed class LoginAdminHandler : IRequestHandler<LoginAdminCommand, AdminLoginResult>
{
    private readonly IAdminAuthService _adminAuthService;

    public LoginAdminHandler(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    public Task<AdminLoginResult> Handle(LoginAdminCommand command, CancellationToken ct)
    {
        return _adminAuthService.LoginAsync(
            new AdminLoginRequest(
                command.Email,
                command.Password,
                command.IpAddress,
                command.UserAgent),
            ct);
    }
}
