using DrakkarVpn.Core.Api.Modules.Users.Application.Users.Commands;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Users.Validation;

public sealed class ChangeStatusValidator : AbstractValidator<ChangeStatus>
{
    public ChangeStatusValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}