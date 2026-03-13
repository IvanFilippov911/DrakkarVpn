using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionQueryService
{
    Task<GetActiveSubscriptionDto?> GetActiveByUserAsync(
        Guid userId,
        DateTime nowUtc,
        CancellationToken ct);

    Task<Dictionary<Guid, SubscriptionSummaryRow>> GetSummariesForUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct);
    
    Task<IReadOnlyList<Guid>> GetExpiredActiveIdsAsync(
        DateTime nowUtc,
        CancellationToken ct = default);
}