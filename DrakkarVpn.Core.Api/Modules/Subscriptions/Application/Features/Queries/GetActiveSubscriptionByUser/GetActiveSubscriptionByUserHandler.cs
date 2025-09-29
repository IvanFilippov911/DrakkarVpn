using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;

public sealed class GetActiveSubscriptionByUserHandler 
    : IRequestHandler<GetActiveSubscriptionByUserRequest, GetActiveSubscriptionDto?>
{
    private readonly ISubscriptionRepository _repository;

    public GetActiveSubscriptionByUserHandler(ISubscriptionRepository repository)
        => _repository = repository;

    public async Task<GetActiveSubscriptionDto?> Handle(GetActiveSubscriptionByUserRequest request, CancellationToken ct)
    {
        var subscription = await _repository.GetActiveByUserAsync(request.UserId, ct);
        if (subscription is null) return null;

        return new GetActiveSubscriptionDto(
            subscription.Id.Value,
            subscription.TariffId.Value,
            subscription.StartAt,
            subscription.EndAt
        );
    }
}