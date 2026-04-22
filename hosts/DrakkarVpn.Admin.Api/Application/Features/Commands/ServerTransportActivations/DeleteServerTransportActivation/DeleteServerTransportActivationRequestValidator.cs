using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;

public sealed class DeleteServerTransportActivationRequestValidator : AbstractValidator<DeleteServerTransportActivationRequest>
{
    public DeleteServerTransportActivationRequestValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();

        RuleFor(x => x.ActivationId)
            .NotEmpty();
    }
}
