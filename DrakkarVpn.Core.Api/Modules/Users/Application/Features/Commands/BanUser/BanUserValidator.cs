using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.BanUser;

public sealed class BanUserValidator : AbstractValidator<BanUserCommand>
{
    public BanUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}