using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminMarkUserInternal;

public sealed record AdminMarkUserInternalCommand(Guid UserId, bool IsInternal) : IRequest<bool>, IUsersCommand<bool>;