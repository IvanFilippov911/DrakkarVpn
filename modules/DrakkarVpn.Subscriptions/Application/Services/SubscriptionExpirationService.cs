using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services;

public sealed class SubscriptionExpirationService : ISubscriptionExpirationService
{
    private readonly ISubscriptionRepository _subs;

    public SubscriptionExpirationService(ISubscriptionRepository subs) => _subs = subs;

    public async Task<ExpireSubscriptionsResultDto> ExpireManyExistingAsync(
        IReadOnlyCollection<Guid> existingSubscriptionIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = existingSubscriptionIds.Where(x => x != Guid.Empty).Distinct().ToArray();
        if (ids.Length == 0) return ExpireSubscriptionsResultDto.Empty();

        await _subs.ExpireManyAsync(ids, markerUtc, ct);

        var succeeded = await _subs.GetByStatusMarkerAsync(ids, markerUtc, SubscriptionStatus.Expired, ct);
        if (succeeded.Count == 0) return ExpireSubscriptionsResultDto.Failed(ids);

        var ok = succeeded.ToHashSet();
        var failed = ids.Where(x => !ok.Contains(x)).ToArray();

        return failed.Length == 0
            ? ExpireSubscriptionsResultDto.Succeeded(succeeded)
            : ExpireSubscriptionsResultDto.Partial(succeeded, failed);
    }
}