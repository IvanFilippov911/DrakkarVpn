using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.MarkUserInternal;

public sealed class MarkUserInternalValidator 
    : AbstractValidator<MarkUserInternalCommand>
{
    public MarkUserInternalValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}