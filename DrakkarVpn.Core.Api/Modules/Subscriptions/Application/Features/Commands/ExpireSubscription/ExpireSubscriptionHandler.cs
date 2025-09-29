using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;

public sealed class ExpireSubscriptionHandler : IRequestHandler<ExpireSubscriptionRequest, Unit>
{
    private readonly ISubscriptionRepository _subscriptions;

    public ExpireSubscriptionHandler(ISubscriptionRepository subscriptions)
        => _subscriptions = subscriptions;

    public async Task<Unit> Handle(ExpireSubscriptionRequest request, CancellationToken ct)
    {
        var subscription = await _subscriptions.GetByIdAsync(new SubscriptionId(request.SubscriptionId), ct);
        if (subscription is null)
            return Unit.Value;

        subscription.Expire();
        return Unit.Value;
    }
}