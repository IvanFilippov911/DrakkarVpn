using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;

public sealed class RegisterServerRequestValidator
    : AbstractValidator<RegisterServerRequest>
{
    public RegisterServerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Region).NotEmpty();
        RuleFor(x => x.PublicHost).NotEmpty();
        RuleFor(x => x.AgentBaseUrl).NotEmpty();
        RuleFor(x => x.AgentTokenEncrypted).NotEmpty();
        RuleFor(x => x.MaxPeers).GreaterThan(0).When(x => x.MaxPeers.HasValue);
    }
}