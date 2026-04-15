using DrakkarVpn.Admin.Api.Application.Features.Commands.Servers.RegisterServer;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;

public sealed class RegisterServerRequestValidator
    : AbstractValidator<RegisterServerRequest>
{
    public RegisterServerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Region)
            .NotEmpty();

        RuleFor(x => x.PublicHost)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.PublicPort)
            .InclusiveBetween(1, 65535);

        RuleFor(x => x.AgentBaseUrl)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("AgentBaseUrl must be a valid absolute URI.");

        RuleFor(x => x.AgentTokenEncrypted)
            .NotEmpty();

        RuleFor(x => x.MaxPeers)
            .GreaterThan(0)
            .When(x => x.MaxPeers.HasValue);
    }
}