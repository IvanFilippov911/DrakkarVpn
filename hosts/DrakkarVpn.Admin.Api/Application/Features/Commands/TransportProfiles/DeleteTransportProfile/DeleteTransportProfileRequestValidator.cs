using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DeleteTransportProfile;

public sealed class DeleteTransportProfileRequestValidator : AbstractValidator<DeleteTransportProfileRequest>
{
    public DeleteTransportProfileRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty();
    }
}
