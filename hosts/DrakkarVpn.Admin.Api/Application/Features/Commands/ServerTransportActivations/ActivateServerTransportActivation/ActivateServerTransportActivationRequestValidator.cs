using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed class ActivateServerTransportActivationRequestValidator : AbstractValidator<ActivateServerTransportActivationRequest>
{
    public ActivateServerTransportActivationRequestValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();

        RuleFor(x => x.ActivationId)
            .NotEmpty();
    }
}
