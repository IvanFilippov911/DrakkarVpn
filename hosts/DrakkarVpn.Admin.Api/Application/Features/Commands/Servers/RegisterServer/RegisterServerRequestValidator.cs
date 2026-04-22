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

        RuleForEach(x => x.TransportProfiles!)
            .ChildRules(profile =>
            {
                profile.RuleFor(x => x.TransportProfileId)
                    .NotEmpty();

                profile.RuleFor(x => x.RealityPublicKey)
                    .NotEmpty()
                    .MaximumLength(512);

                profile.RuleFor(x => x.LocalPriority)
                    .GreaterThanOrEqualTo(0);
            })
            .When(x => x.TransportProfiles is not null);

        RuleFor(x => x.TransportProfiles)
            .Must(items => items is null || items.Select(i => i.TransportProfileId).Distinct().Count() == items.Count)
            .WithMessage("TransportProfiles must not contain duplicate TransportProfileId values.");

        RuleFor(x => x)
            .Must(x =>
            {
                if (!x.ActivateProfileId.HasValue)
                    return true;

                if (x.TransportProfiles is null || x.TransportProfiles.Count == 0)
                    return false;

                return x.TransportProfiles.Any(p => p.TransportProfileId == x.ActivateProfileId.Value);
            })
            .WithMessage("ActivateProfileId must reference one of TransportProfiles.");
    }
}