using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task<IReadOnlyList<Subscription>> GetExpiredActiveAsync(DateTime now, CancellationToken ct = default);
    Task DeleteAsync(Subscription subscription, CancellationToken ct = default);

    Task<Dictionary<Guid, SubscriptionSummaryRow>> GetSummariesForUsersAsync(
        IReadOnlyCollection<Guid> userIds, CancellationToken ct);
}