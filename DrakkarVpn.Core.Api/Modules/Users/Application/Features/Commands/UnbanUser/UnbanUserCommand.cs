using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.UnbanUser;

public sealed record UnbanUserCommand(Guid UserId) : IRequest<bool>;