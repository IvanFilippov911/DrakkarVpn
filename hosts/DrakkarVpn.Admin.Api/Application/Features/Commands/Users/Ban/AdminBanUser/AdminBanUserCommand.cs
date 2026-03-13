using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBanUser;

public sealed record AdminBanUserCommand(
    Guid UserId,
    string? Reason
) : IRequest;