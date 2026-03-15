using DrakkarVpn.AdminAuth.Application.Abstractions;
using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LoginAdmin;

public sealed record LoginAdminCommand(
    string Email,
    string Password,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<AdminLoginResult>, IAdminAuthCommand<AdminLoginResult>;
