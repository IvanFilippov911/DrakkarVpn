using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerById;

public sealed class GetServerByIdRequestValidator
    : AbstractValidator<GetServerByIdRequest>
{
    public GetServerByIdRequestValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
    }
}