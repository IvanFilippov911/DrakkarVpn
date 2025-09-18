using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;

public sealed class RegisterServerValidator : AbstractValidator<RegisterServerRequest>
{
    public RegisterServerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Region)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PublicHost)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.AgentBaseUrl)
            .NotEmpty()
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("AgentBaseUrl must be a valid absolute URL");

        RuleFor(x => x.AgentTokenEncrypted)
            .NotEmpty();

        RuleFor(x => x.MaxPeers)
            .Must(v => v is null or >= 0);
    }
}