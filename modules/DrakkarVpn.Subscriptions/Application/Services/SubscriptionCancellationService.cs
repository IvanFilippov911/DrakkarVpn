using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services;

public sealed class SubscriptionCancellationService : ISubscriptionCancellationService
{
    private readonly ISubscriptionRepository _subs;

    public SubscriptionCancellationService(ISubscriptionRepository subs)
        => _subs = subs;

    public async Task<CancelSubscriptionsResultDto> CancelManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = subscriptionIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
            return CancelSubscriptionsResultDto.Empty();

        await _subs.CancelManyAsync(ids, markerUtc, ct);

        var succeeded = await _subs.GetByStatusMarkerAsync(
            ids, markerUtc, SubscriptionStatus.Cancelled, ct);

        if (succeeded.Count == 0)
            return CancelSubscriptionsResultDto.AllFailed(ids);

        var okSet = succeeded.ToHashSet();
        var failed = ids.Where(id => !okSet.Contains(id)).ToList();

        return failed.Count == 0
            ? CancelSubscriptionsResultDto.AllSucceeded(succeeded)
            : CancelSubscriptionsResultDto.Partial(succeeded, failed);
    }
}