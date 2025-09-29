using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.RenewSubscription;

public sealed class RenewSubscriptionHandler : IRequestHandler<RenewSubscriptionRequest, Unit>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ITariffRepository _tariffs;

    public RenewSubscriptionHandler(
        ISubscriptionRepository repository,
        ITariffRepository tariffs)
    {
        _repository = repository;
        _tariffs = tariffs;
    }

    public async Task<Unit> Handle(RenewSubscriptionRequest request, CancellationToken ct)
    {
        var subscription = await _repository.GetByIdAsync(new(request.SubscriptionId), ct);
        if (subscription is null)
            throw new InvalidOperationException($"Subscription {request.SubscriptionId} not found");

        var tariff = await _tariffs.GetByIdAsync(subscription.TariffId, ct);
        if (tariff is null || tariff.Status != TariffStatus.Active)
            throw new InvalidOperationException($"Tariff {subscription.TariffId.Value} not available");

        var newEndAt = subscription.IsActive()
            ? subscription.EndAt.Add(tariff.Duration)
            : DateTime.UtcNow.Add(tariff.Duration);

        subscription.Renew(newEndAt);

        return Unit.Value;
    }
}