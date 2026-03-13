using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerPeersOnlineHistory;

public sealed class GetServerPeersOnlineHistoryQueryValidator
    : AbstractValidator<GetServerPeersOnlineHistoryQuery>
{
    public GetServerPeersOnlineHistoryQueryValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Minutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(7 * 24 * 60);
    }
}