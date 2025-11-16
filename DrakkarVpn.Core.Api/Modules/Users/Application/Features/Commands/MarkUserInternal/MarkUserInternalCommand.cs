using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.MarkUserInternal;

public sealed record MarkUserInternalCommand(
    Guid UserId,
    bool IsInternal
) : IRequest<bool>;