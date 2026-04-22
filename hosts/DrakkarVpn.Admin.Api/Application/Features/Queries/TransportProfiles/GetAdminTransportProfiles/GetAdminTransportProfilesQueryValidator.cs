using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.TransportProfiles.GetAdminTransportProfiles;

public sealed class GetAdminTransportProfilesQueryValidator : AbstractValidator<GetAdminTransportProfilesQuery>
{
    public GetAdminTransportProfilesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be >= 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("PageSize must be between 1 and 200");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.SortBy)
            .IsInEnum();

        RuleFor(x => x.SortDirection)
            .IsInEnum();
    }
}
