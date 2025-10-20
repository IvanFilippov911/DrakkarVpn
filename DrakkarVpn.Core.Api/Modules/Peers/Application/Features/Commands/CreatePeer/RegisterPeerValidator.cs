using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;

public sealed class RegisterPeerValidator : AbstractValidator<RegisterPeerRequest>
{
    public RegisterPeerValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ServerId).NotEmpty();
    }
}