using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RevokeAllServerPeers;

public sealed class RevokeAllServerPeersCommandValidator
    : AbstractValidator<RevokeAllServerPeersCommand>
{
    public RevokeAllServerPeersCommandValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
    }
}