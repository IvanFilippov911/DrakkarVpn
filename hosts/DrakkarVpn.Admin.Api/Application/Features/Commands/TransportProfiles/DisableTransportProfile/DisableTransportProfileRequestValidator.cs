using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DisableTransportProfile;

public sealed class DisableTransportProfileRequestValidator : AbstractValidator<DisableTransportProfileRequest>
{
    public DisableTransportProfileRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty();
    }
}
