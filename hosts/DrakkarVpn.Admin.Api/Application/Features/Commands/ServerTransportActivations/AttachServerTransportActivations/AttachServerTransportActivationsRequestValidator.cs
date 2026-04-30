using DrakkarVpn.Servers.Domain.Entities;
using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransportActivations;

public sealed class AttachServerTransportActivationsRequestValidator : AbstractValidator<AttachServerTransportActivationsRequest>
{
    public AttachServerTransportActivationsRequestValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();

        RuleFor(x => x.Profiles)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Profiles)
            .ChildRules(profile =>
            {
                profile.RuleFor(x => x.TransportProfileId)
                    .NotEmpty();

                profile.RuleFor(x => x.RealityPublicKey)
                    .NotEmpty()
                    .MaximumLength(ServerTransportProfileActivation.RealityPublicKeyMaxLength);

                profile.RuleFor(x => x.LocalPriority)
                    .GreaterThanOrEqualTo(0);
            });

        RuleFor(x => x.Profiles)
            .Must(items => items.Select(i => i.TransportProfileId).Distinct().Count() == items.Count)
            .WithMessage("Profiles must not contain duplicate TransportProfileId values.");

        RuleFor(x => x)
            .Must(x =>
            {
                if (!x.ActivateProfileId.HasValue)
                    return true;

                return x.Profiles.Any(p => p.TransportProfileId == x.ActivateProfileId.Value);
            })
            .WithMessage("ActivateProfileId must reference one of Profiles.");
    }
}
