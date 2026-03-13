using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreErrorEvents;

public sealed class GetCoreErrorEventsValidator 
    : AbstractValidator<GetCoreErrorEventsQuery>
{
    public GetCoreErrorEventsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200);

        RuleFor(x => x.FromUtc)
            .LessThan(x => x.ToUtc)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue);
        
    }
}