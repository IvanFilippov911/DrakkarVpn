using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;

public sealed class RegisterOrGetByTelegramValidator 
    : AbstractValidator<RegisterRequest>
{
    public RegisterOrGetByTelegramValidator()
    {
        RuleFor(x => x.TelegramId)
            .GreaterThan(0)
            .WithMessage("TelegramId must be positive.");
    }
}