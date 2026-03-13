using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Peers.RevokePeer;

public sealed class RevokePeerRequestValidator : AbstractValidator<RevokePeerRequest>
{
    public RevokePeerRequestValidator()
    {
        RuleFor(x => x.PeerId).NotEmpty();
        RuleFor(x => x.ServerId).NotEmpty();
    }
}