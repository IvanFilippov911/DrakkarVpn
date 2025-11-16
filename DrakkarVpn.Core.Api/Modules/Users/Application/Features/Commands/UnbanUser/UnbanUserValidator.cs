using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.UnbanUser;

public sealed class UnbanUserValidator : AbstractValidator<UnbanUserCommand>
{
    public UnbanUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}