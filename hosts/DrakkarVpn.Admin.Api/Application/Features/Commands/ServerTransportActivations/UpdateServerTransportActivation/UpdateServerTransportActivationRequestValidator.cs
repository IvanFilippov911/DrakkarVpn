using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Entities;
using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;

public sealed class UpdateServerTransportActivationRequestValidator : AbstractValidator<UpdateServerTransportActivationRequest>
{
    public UpdateServerTransportActivationRequestValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();

        RuleFor(x => x.ActivationId)
            .NotEmpty();

        RuleFor(x => x.RealityPublicKey)
            .NotEmpty()
            .MaximumLength(ServerTransportProfileActivation.RealityPublicKeyMaxLength);

        RuleFor(x => x.LocalPriority)
            .GreaterThanOrEqualTo(0);
    }
}
