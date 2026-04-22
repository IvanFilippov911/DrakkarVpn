using FluentValidation;
using NetworkMonitoring.Application.DTOs;
using NetworkMonitoring.Domain;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.UpdateProbeNode;

public sealed class UpdateProbeNodeRequestValidator : AbstractValidator<UpdateProbeNodeRequest>
{
    public UpdateProbeNodeRequestValidator()
    {
        RuleFor(x => x.ProbeNodeId)
            .NotEmpty();

        RuleFor(x => x.Data)
            .NotNull();

        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MaximumLength(ProbeNode.NameMaxLength);

        RuleFor(x => x.Data.Region)
            .NotEmpty()
            .MaximumLength(ProbeNode.RegionMaxLength);

        RuleFor(x => x.Data.Host)
            .NotEmpty()
            .MaximumLength(ProbeNode.HostMaxLength);
    }
}

