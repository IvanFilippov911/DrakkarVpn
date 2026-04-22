using FluentValidation;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportActivations;

public sealed class GetServerTransportActivationsQueryValidator : AbstractValidator<GetServerTransportActivationsQuery>
{
    public GetServerTransportActivationsQueryValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();
    }
}
