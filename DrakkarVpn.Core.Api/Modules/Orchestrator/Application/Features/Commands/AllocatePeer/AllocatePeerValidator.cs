using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed class AllocatePeerValidator : AbstractValidator<AllocatePeerRequest>
{
    public AllocatePeerValidator()
    {
        RuleFor(x => x.TelegramId)
            .NotEmpty().WithMessage("TelegramId is required");
    }
}