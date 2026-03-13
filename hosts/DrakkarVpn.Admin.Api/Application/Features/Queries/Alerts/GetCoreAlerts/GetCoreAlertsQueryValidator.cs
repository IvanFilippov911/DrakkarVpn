using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreAlerts;

public sealed class GetCoreAlertsQueryValidator : AbstractValidator<GetCoreAlertsQuery>
{
    public GetCoreAlertsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

        RuleFor(x => x.FromUtc)
            .LessThanOrEqualTo(x => x.ToUtc)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue);
    }
}