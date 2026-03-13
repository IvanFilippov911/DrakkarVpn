using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionExpirationService
{
    Task<ExpireSubscriptionsResultDto> ExpireManyExistingAsync(
        IReadOnlyCollection<Guid> existingSubscriptionIds,
        DateTime markerUtc,
        CancellationToken ct);
}