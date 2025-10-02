using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.DeletePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.DeleteSubscription;

public sealed class DeleteSubscriptionHandler 
    : IRequestHandler<DeleteSubscriptionRequest, bool>
{
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IMediator _mediator;

    public DeleteSubscriptionHandler(
        ISubscriptionRepository subscriptions, 
        IMediator mediator)
    {
        _subscriptions = subscriptions;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteSubscriptionRequest request, CancellationToken ct)
    {
        var subscription = await _subscriptions.GetByIdAsync(new SubscriptionId(request.SubscriptionId), ct);
        if (subscription is null)
            return false;
        
        var peers = await _mediator.Send(new GetPeersBySubscriptionRequest(request.SubscriptionId), ct);

        foreach (var peer in peers)
        {
            await _mediator.Send(new DeletePeerRequest(peer.Id), ct);
        }

        await _subscriptions.DeleteAsync(subscription, ct);
        return true;
    }
}