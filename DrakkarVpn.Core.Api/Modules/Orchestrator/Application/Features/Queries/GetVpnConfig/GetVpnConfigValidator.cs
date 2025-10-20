using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;

public sealed class GetVpnConfigValidator : AbstractValidator<GetVpnConfigRequest>
{
    public GetVpnConfigValidator()
    {
        RuleFor(x => x.TelegramId).GreaterThan(0);
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Platform).MaximumLength(32).When(x => x.Platform != null);
        RuleFor(x => x.DeviceName).MaximumLength(128).When(x => x.DeviceName != null);
        RuleFor(x => x.Region).MaximumLength(32).When(x => x.Region != null);
    }
}