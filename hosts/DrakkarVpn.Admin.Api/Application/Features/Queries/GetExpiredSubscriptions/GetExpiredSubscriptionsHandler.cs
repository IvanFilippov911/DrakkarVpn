using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;

public sealed class GetExpiredSubscriptionsHandler 
    : IRequestHandler<GetExpiredSubscriptionsRequest, IReadOnlyList<Guid>>
{
    private readonly ISubscriptionQueryService _svc;

    public GetExpiredSubscriptionsHandler(ISubscriptionQueryService svc)
        => _svc = svc;

    public async Task<IReadOnlyList<Guid>> Handle(GetExpiredSubscriptionsRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var subs = await _svc.GetExpiredActiveIdsAsync(now, ct); 
        return subs;
    }
}