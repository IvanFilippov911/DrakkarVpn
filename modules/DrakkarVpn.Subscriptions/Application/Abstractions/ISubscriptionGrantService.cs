using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionGrantService
{
    Task<SubscriptionDto> GrantAsync(
        Guid userId,
        Guid tariffId,
        int? deviceCount,
        DateTime nowUtc,
        CancellationToken ct);

    Task<BulkGrantSubscriptionsResultDto> GrantBulkAsync(
        IReadOnlyCollection<Guid> existingUserIds,
        Guid tariffId,
        int? deviceCount,
        DateTime markerUtc,
        CancellationToken ct);
}