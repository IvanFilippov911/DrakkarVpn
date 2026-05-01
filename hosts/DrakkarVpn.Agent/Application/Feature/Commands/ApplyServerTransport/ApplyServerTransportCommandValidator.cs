using FluentValidation;

namespace DrakkarVpn.Agent.Application.Feature.Commands.ApplyServerTransport;

public sealed class ApplyServerTransportCommandValidator : AbstractValidator<ApplyServerTransportCommand>
{
    public ApplyServerTransportCommandValidator()
    {
        RuleFor(x => x.Body)
            .NotNull()
            .WithErrorCode("body_required")
            .WithMessage("body required");

        When(cmd => cmd.Body is not null, () =>
        {
            RuleFor(x => x.Body!.OperationId)
                .NotEmpty()
                .WithErrorCode("invalid_operation_id")
                .WithMessage("operationId is required");
            RuleFor(x => x.Body!.ServerId)
                .NotEmpty()
                .WithErrorCode("invalid_server_id")
                .WithMessage("serverId is required");
            RuleFor(x => x.Body!.ActivationId)
                .NotEmpty()
                .WithErrorCode("invalid_activation_id")
                .WithMessage("activationId is required");
            RuleFor(x => x.Body!.PublicPort)
                .InclusiveBetween(1, 65535)
                .WithErrorCode("invalid_public_port")
                .WithMessage("publicPort must be between 1 and 65535");
            RuleFor(x => x.Body!.PublicHost)
                .Must(NotBlank)
                .WithErrorCode("invalid_public_host")
                .WithMessage("publicHost is required");
            RuleFor(x => x.Body!.TransportType)
                .Must(NotBlank)
                .WithErrorCode("invalid_transport_type")
                .WithMessage("transportType is required");
            RuleFor(x => x.Body!.SecurityType)
                .Must(NotBlank)
                .WithErrorCode("invalid_security_type")
                .WithMessage("securityType is required");
            RuleFor(x => x.Body!.RealitySni)
                .Must(NotBlank)
                .WithErrorCode("invalid_reality_sni")
                .WithMessage("realitySni is required");
            RuleFor(x => x.Body!.RealityShortId)
                .Must(NotBlank)
                .WithErrorCode("invalid_reality_short_id")
                .WithMessage("realityShortId is required");
            RuleFor(x => x.Body!.RealityFingerprint)
                .Must(NotBlank)
                .WithErrorCode("invalid_reality_fingerprint")
                .WithMessage("realityFingerprint is required");
            RuleFor(x => x.Body!.RealityPublicKey)
                .Must(NotBlank)
                .WithErrorCode("invalid_reality_public_key")
                .WithMessage("realityPublicKey is required");
            RuleFor(x => x.Body!.InboundTag)
                .Must(NotBlank)
                .WithErrorCode("invalid_inbound_tag")
                .WithMessage("inboundTag is required");
            RuleFor(x => x.Body!.Flow)
                .Must(NotBlank)
                .WithErrorCode("invalid_flow")
                .WithMessage("flow is required");
            RuleFor(x => x.Body!.Encryption)
                .Must(NotBlank)
                .WithErrorCode("invalid_encryption")
                .WithMessage("encryption is required");
        });
    }

    private static bool NotBlank(string? value) => !string.IsNullOrWhiteSpace(value?.Trim());
}
