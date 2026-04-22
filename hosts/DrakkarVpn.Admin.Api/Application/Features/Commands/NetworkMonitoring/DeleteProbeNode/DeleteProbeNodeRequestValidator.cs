using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.DeleteProbeNode;

public sealed class DeleteProbeNodeRequestValidator : AbstractValidator<DeleteProbeNodeRequest>
{
    public DeleteProbeNodeRequestValidator()
    {
        RuleFor(x => x.ProbeNodeId)
            .NotEmpty();
    }
}

