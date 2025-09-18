using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpdateServerHealth;

public sealed class UpdateServerHealthValidator : AbstractValidator<UpdateServerHealthRequest>
{
    public UpdateServerHealthValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.PeersActive).GreaterThanOrEqualTo(0);
    }
}