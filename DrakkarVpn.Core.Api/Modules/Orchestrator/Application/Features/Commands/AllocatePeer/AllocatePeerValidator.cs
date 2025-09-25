using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed class AllocatePeerValidator : AbstractValidator<AllocatePeerRequest>
{
    public AllocatePeerValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}