using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.BanUser;

public sealed record BanUserCommand(
    Guid   UserId,
    string Reason
) : IRequest<bool>;