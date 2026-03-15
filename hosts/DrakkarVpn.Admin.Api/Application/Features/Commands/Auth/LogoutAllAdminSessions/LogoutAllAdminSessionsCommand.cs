using DrakkarVpn.AdminAuth.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LogoutAllAdminSessions;

public sealed record LogoutAllAdminSessionsCommand : IRequest<Unit>, IAdminAuthCommand<Unit>;
