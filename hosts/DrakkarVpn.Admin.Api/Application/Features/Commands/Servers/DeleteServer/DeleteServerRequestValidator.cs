using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.DeleteServer;

public sealed class DeleteServerRequestValidator
    : AbstractValidator<DeleteServerRequest>
{
    public DeleteServerRequestValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
    }
}