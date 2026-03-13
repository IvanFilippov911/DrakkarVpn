using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminUnbanUser;

public sealed record AdminUnbanUserCommand(Guid UserId) : IUsersCommand;