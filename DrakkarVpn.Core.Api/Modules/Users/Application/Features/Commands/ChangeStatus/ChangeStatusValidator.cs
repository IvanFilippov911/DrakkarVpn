using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeStatus;

public sealed class ChangeStatusValidator : AbstractValidator<ChangeStatusRequest>
{
    public ChangeStatusValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}