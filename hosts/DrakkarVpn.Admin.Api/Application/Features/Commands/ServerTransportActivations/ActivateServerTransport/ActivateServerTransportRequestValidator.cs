using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed class ActivateServerTransportRequestValidator : AbstractValidator<ActivateServerTransportRequest>
{
    public ActivateServerTransportRequestValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();

        RuleFor(x => x.ActivationId)
            .NotEmpty();
    }
}
