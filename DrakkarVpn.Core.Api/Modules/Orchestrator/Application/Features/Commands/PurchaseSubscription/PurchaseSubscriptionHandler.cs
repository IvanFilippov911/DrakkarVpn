using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.CreateSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.RenewSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;

public sealed class PurchaseSubscriptionHandler
    : IRequestHandler<PurchaseSubscriptionRequest, Guid>
{
    private readonly IMediator _mediator;

    public PurchaseSubscriptionHandler(IMediator mediator) => _mediator = mediator;

    public async Task<Guid> Handle(PurchaseSubscriptionRequest req, CancellationToken ct)
    {
        var user = await _mediator.Send(new GetUserByTelegramIdRequest(req.TelegramId), ct);
        
        SubscriptionDto subscription;

        var activeSub = await _mediator.Send(new GetActiveSubscriptionByUserRequest(user.Id), ct);

        if (activeSub is not null)
        {
            subscription = await _mediator.Send(new RenewSubscriptionRequest(activeSub.Id), ct);
        }
        else
        {
            subscription = await _mediator.Send(new CreateSubscriptionRequest(user.Id, req.TariffId), ct);
        }
        
        await _mediator.Send(
            new AllocatePeerRequest(
                req.TelegramId, 
                req.Region, 
                subscription.Id, 
                subscription.EndAt
            ),
            ct
        );

        return subscription.Id;
    }
}