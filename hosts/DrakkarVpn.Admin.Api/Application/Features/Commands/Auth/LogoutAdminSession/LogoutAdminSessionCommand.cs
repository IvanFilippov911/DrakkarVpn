using DrakkarVpn.AdminAuth.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LogoutAdminSession;

public sealed record LogoutAdminSessionCommand(string RefreshToken) : IRequest<Unit>, IAdminAuthCommand<Unit>;
