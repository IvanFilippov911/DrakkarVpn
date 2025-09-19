using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;

public sealed class RevokePeerValidator : AbstractValidator<RevokePeerRequest>
{
    public RevokePeerValidator()
    {
        RuleFor(x => x.PeerId).NotEmpty();
        RuleFor(x => x.ServerId).NotEmpty();
    }
}