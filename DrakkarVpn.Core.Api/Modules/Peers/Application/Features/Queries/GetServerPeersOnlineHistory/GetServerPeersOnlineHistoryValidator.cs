
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetServerPeersOnlineHistory;

public sealed class GetServerPeersOnlineHistoryValidator 
    : AbstractValidator<GetServerPeersOnlineHistoryQuery>
{
    public GetServerPeersOnlineHistoryValidator()
    {
        RuleFor(x => x.ServerId)
            .NotEmpty();

        RuleFor(x => x.Minutes)
            .InclusiveBetween(10, 7 * 24 * 60)
            .WithMessage("Minutes must be between 10 and 10080.");
    }
}