using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums;
using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.CreateTransportProfile;

public sealed class CreateTransportProfileRequestValidator : AbstractValidator<CreateTransportProfileRequest>
{
    public CreateTransportProfileRequestValidator()
    {
        RuleFor(x => x.Data)
            .NotNull();

        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MaximumLength(TransportProfile.NameMaxLength);

        RuleFor(x => x.Data.TransportType)
            .IsInEnum();

        RuleFor(x => x.Data.SecurityType)
            .Equal(SecurityType.Reality)
            .WithMessage("Only Reality security type is supported.");

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
            .NotEmpty()
            .MaximumLength(TransportProfile.GrpcFieldMaxLength)
            .When(x => x.Data.TransportType == TransportType.Grpc);

        RuleFor(x => x.Data.GrpcAuthority)
            .MaximumLength(TransportProfile.GrpcFieldMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Data.GrpcAuthority));

        RuleFor(x => x.Data.GlobalPriority)
            .GreaterThanOrEqualTo(0);
    }
}
