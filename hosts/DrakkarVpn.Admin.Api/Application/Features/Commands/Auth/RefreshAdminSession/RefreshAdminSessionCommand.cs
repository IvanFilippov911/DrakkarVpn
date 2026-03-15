using DrakkarVpn.AdminAuth.Application.Abstractions;
using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.RefreshAdminSession;

public sealed record RefreshAdminSessionCommand(
    string RefreshToken,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<AdminRefreshSessionResult>, IAdminAuthCommand<AdminRefreshSessionResult>;
