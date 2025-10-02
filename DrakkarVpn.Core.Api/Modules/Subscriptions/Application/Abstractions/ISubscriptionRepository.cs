using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Subscription?> GetByIdAsync(SubscriptionId id, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task<IReadOnlyList<Subscription>> GetExpiredActiveAsync(DateTime now, CancellationToken ct = default);
    Task DeleteAsync(Subscription subscription, CancellationToken ct = default);
}