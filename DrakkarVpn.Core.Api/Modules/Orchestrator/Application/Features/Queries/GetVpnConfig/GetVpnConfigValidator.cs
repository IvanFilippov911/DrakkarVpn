using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;

public sealed class GetVpnConfigValidator : AbstractValidator<GetVpnConfigRequest>
{
    public GetVpnConfigValidator()
    {
        RuleFor(x => x.TelegramId).GreaterThan(0);
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Region).MaximumLength(32).When(x => x.Region != null);
    }
}