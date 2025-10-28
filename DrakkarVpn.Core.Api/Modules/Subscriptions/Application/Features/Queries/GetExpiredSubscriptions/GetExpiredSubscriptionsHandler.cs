using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;

public sealed class GetExpiredSubscriptionsHandler 
    : IRequestHandler<GetExpiredSubscriptionsRequest, IReadOnlyList<ExpiredSubscriptionDto>>
{
    private readonly ISubscriptionRepository _repository;

    public GetExpiredSubscriptionsHandler(ISubscriptionRepository repository)
        => _repository = repository;

    public async Task<IReadOnlyList<ExpiredSubscriptionDto>> Handle(GetExpiredSubscriptionsRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var subs = await _repository.GetExpiredActiveAsync(now, ct);

        return subs.Select(s => new ExpiredSubscriptionDto(
            s.Id,
            s.UserId,
            s.EndAt
        )).ToList();
    }
}