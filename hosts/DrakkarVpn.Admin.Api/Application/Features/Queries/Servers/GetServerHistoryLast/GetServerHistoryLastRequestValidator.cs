using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerHistoryLast;

public sealed class GetServerHistoryLastRequestValidator
    : AbstractValidator<GetServerHistoryLastRequest>
{
    public GetServerHistoryLastRequestValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Minutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(7 * 24 * 60);
    }
}