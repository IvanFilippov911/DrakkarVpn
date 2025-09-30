using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;

public sealed class ExpireSubscriptionHandler : IRequestHandler<ExpireSubscriptionRequest, Unit>
{
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IMediator _mediator;

    public ExpireSubscriptionHandler(
        ISubscriptionRepository subscriptions,
        IMediator mediator)
    {
        _subscriptions = subscriptions;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(ExpireSubscriptionRequest request, CancellationToken ct)
    {
        var subscription = await _subscriptions.GetByIdAsync(new SubscriptionId(request.SubscriptionId), ct);
        if (subscription is null)
            return Unit.Value;
        
        subscription.Expire();

        var peers = await _mediator.Send(new GetPeersBySubscriptionRequest(subscription.Id.Value), ct);

        foreach (var peer in peers.Where(p => p.Status == PeerStatus.Active))
        {
            await _mediator.Send(new RevokePeerRequest(peer.Id, peer.ServerId), ct);
        }

        return Unit.Value;
    }
}