using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionRepository
{
    Task<Dictionary<Guid, ActiveSubscriptionRow>> GetActiveByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime nowUtc,
        CancellationToken ct);
    
    Task<Subscription?> GetActiveByUserAsync(Guid userId, DateTime nowUtc, CancellationToken ct);
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task<IReadOnlyList<Subscription>> GetExpiredActiveAsync(DateTime now, CancellationToken ct = default);
    Task DeleteAsync(Subscription subscription, CancellationToken ct = default);

    Task<Dictionary<Guid, SubscriptionSummaryRow>> GetSummariesForUsersAsync(
        IReadOnlyCollection<Guid> userIds, CancellationToken ct);
    
    Task<IReadOnlyList<SubscriptionUserRow>> GetUsersBySubscriptionIdsAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        CancellationToken ct);
    
    Task<IReadOnlyList<SubscriptionUserRow>> GetExistingByIdsAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        CancellationToken ct);

    Task CancelManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        CancellationToken ct);

    Task ExpireManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        CancellationToken ct);
    
    Task RenewManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime nowUtc,
        TimeSpan duration,
        int maxDevices,
        DateTime markerUtc,
        CancellationToken ct);

    Task CreateManyAsync(
        IReadOnlyCollection<Subscription> subs,
        CancellationToken ct);

    Task<IReadOnlyList<Guid>> GetByStatusMarkerAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        SubscriptionStatus expectedStatus,
        CancellationToken ct);
}