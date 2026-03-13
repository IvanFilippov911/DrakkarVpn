using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerDetails;

public sealed class GetPeerDetailsQueryValidator : AbstractValidator<GetPeerDetailsQuery>
{
    public GetPeerDetailsQueryValidator()
    {
        RuleFor(x => x.PeerId).NotEmpty();
    }
}