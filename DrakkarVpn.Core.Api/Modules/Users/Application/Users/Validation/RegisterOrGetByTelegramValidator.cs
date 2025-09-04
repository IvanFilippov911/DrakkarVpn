using DrakkarVpn.Core.Api.Modules.Users.Application.Users.Commands;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Validation;

public sealed class RegisterOrGetByTelegramValidator 
    : AbstractValidator<RegisterOrGetByTelegram>
{
    public RegisterOrGetByTelegramValidator()
    {
        RuleFor(x => x.TelegramId)
            .GreaterThan(0)
            .WithMessage("TelegramId must be positive.");
    }
}