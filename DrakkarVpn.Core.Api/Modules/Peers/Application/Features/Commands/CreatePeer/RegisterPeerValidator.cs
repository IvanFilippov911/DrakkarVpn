using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;

public sealed class RegisterPeerValidator : AbstractValidator<RegisterPeerRequest>
{
    public RegisterPeerValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ServerId).NotEmpty();

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future");
    }
}