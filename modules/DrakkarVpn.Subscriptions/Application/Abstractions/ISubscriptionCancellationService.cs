using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionCancellationService
{
    Task<CancelSubscriptionsResultDto> CancelManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        CancellationToken ct);
}