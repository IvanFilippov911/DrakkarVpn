using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.EnableTransportProfile;

public sealed class EnableTransportProfileRequestValidator : AbstractValidator<EnableTransportProfileRequest>
{
    public EnableTransportProfileRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty();
    }
}
