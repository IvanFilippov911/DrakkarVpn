using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RenewPeer;

public sealed class RenewPeerValidator : AbstractValidator<RenewPeerRequest>
{
    public RenewPeerValidator()
    {
        RuleFor(x => x.PeerId).NotEmpty();
        RuleFor(x => x.NewExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("New expiration date must be in the future");
    }
}