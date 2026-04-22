using DrakkarVpn.Servers.Domain.Aggregates;
using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.UpdateTransportProfile;

public sealed class UpdateTransportProfileRequestValidator : AbstractValidator<UpdateTransportProfileRequest>
{
    public UpdateTransportProfileRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty();

        RuleFor(x => x.Data)
            .NotNull();

        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MaximumLength(TransportProfile.NameMaxLength);

        RuleFor(x => x.Data.RealitySni)
            .NotEmpty()
            .MaximumLength(TransportProfile.RealitySniMaxLength);

        RuleFor(x => x.Data.RealityShortId)
            .NotEmpty()
            .MaximumLength(TransportProfile.RealityShortIdMaxLength);

        RuleFor(x => x.Data.RealityFingerprint)
            .NotEmpty()
            .MaximumLength(TransportProfile.RealityFingerprintMaxLength);

        RuleFor(x => x.Data.RealityDest)
            .NotEmpty()
            .MaximumLength(TransportProfile.RealityDestMaxLength);

        RuleFor(x => x.Data.GrpcServiceName)
            .MaximumLength(TransportProfile.GrpcFieldMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Data.GrpcServiceName));

        RuleFor(x => x.Data.GrpcAuthority)
            .MaximumLength(TransportProfile.GrpcFieldMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Data.GrpcAuthority));

        RuleFor(x => x.Data.GlobalPriority)
            .GreaterThanOrEqualTo(0);
    }
}
