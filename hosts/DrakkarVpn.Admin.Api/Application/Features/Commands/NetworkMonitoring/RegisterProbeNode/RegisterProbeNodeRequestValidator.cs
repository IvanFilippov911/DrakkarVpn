using FluentValidation;
using NetworkMonitoring.Domain;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.RegisterProbeNode;

public sealed class RegisterProbeNodeRequestValidator
    : AbstractValidator<RegisterProbeNodeRequest>
{
    public RegisterProbeNodeRequestValidator()
    {
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

