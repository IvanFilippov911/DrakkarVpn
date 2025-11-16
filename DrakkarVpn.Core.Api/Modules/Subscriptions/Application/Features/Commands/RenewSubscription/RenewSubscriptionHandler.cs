using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeSubscriptionDevicesLimit;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.RenewSubscription;

public sealed class RenewSubscriptionHandler 
    : IRequestHandler<RenewSubscriptionRequest, SubscriptionDto>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ITariffRepository _tariffs;
    private readonly IMediator _mediator;

    public RenewSubscriptionHandler(
        ISubscriptionRepository repository,
        ITariffRepository tariffs,
        IMediator mediator)
    {
        _repository = repository;
        _tariffs    = tariffs;
        _mediator   = mediator;
    }

    public async Task<SubscriptionDto> Handle(RenewSubscriptionRequest request, CancellationToken ct)
    {
        var subscription = await _repository.GetByIdAsync(request.SubscriptionId, ct)
                           ?? throw new InvalidOperationException(
                               $"Subscription {request.SubscriptionId} not found");

        var tariff = await _tariffs.GetByIdAsync(new(request.TariffId), ct);
        if (tariff is null || tariff.Status != TariffStatus.Active)
            throw new InvalidOperationException($"Tariff {request.TariffId} not available");

        var newEndAt = subscription.IsActive()
            ? subscription.EndAt.Add(tariff.Duration)
            : DateTime.UtcNow.Add(tariff.Duration);

        subscription.Renew(newEndAt);
        
        if (request.DeviceCount is not null &&
            request.DeviceCount.Value != subscription.MaxDevices)
        {
            await _mediator.Send(
                new ChangeSubscriptionDevicesLimitCommand(
                    SubscriptionId: subscription.Id,
                    NewMaxDevices:  request.DeviceCount.Value
                ),
                ct);
        }

        return new SubscriptionDto(
            subscription.Id,
            subscription.UserId,
            subscription.StartAt,
            subscription.EndAt,
            subscription.Status.ToString(),
            subscription.MaxDevices
        );
    }
}