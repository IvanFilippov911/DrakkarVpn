using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.ChangeServerStatus;

public sealed class ChangeServerStatusValidator : AbstractValidator<ChangeServerStatusRequest>
{
    public ChangeServerStatusValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}