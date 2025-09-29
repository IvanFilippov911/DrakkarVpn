using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.CreateSubscription;

public sealed class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionRequest, Guid>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ITariffRepository _tariffs;

    public CreateSubscriptionHandler(ISubscriptionRepository repository, ITariffRepository tariffs)
    {
        _repository = repository;
        _tariffs = tariffs;
    }

    public async Task<Guid> Handle(CreateSubscriptionRequest request, CancellationToken ct)
    {
        var tariff = await _tariffs.GetByIdAsync(new(request.TariffId), ct);
        if (tariff is null || tariff.Status != TariffStatus.Active)
            throw new InvalidOperationException($"Tariff {request.TariffId} not available");
        
        var startAt = DateTime.UtcNow;
        var endAt = startAt.Add(tariff.Duration);
        
        var subscription = Subscription.CreateNew(request.UserId, tariff.Id, startAt, endAt);

        await _repository.AddAsync(subscription, ct);

        return subscription.Id.Value;
    }
}