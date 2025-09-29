using DrakkarVpn.Shared.Users;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;

public sealed class RegisterOrGetByTelegramValidator 
    : AbstractValidator<RegisterUserRequest>
{
    public RegisterOrGetByTelegramValidator()
    {
        RuleFor(x => x.TelegramId)
            .GreaterThan(0)
            .WithMessage("TelegramId must be positive.");
    }
}