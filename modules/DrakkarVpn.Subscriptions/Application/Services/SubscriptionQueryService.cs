using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services;

public sealed class SubscriptionQueryService : ISubscriptionQueryService
{
    private readonly ISubscriptionRepository _repo;

    public SubscriptionQueryService(ISubscriptionRepository repo) => _repo = repo;

    public async Task<GetActiveSubscriptionDto?> GetActiveByUserAsync(
        Guid userId,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var sub = await _repo.GetActiveByUserAsync(userId, nowUtc, ct);
        if (sub is null) return null;

        return new GetActiveSubscriptionDto(
            sub.Id,
            sub.MaxDevices,
            sub.StartAt,
            sub.EndAt
        );
    }

    public Task<Dictionary<Guid, SubscriptionSummaryRow>> GetSummariesForUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
        => _repo.GetSummariesForUsersAsync(userIds, ct);
    
    public async Task<IReadOnlyList<Guid>> GetExpiredActiveIdsAsync(
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        var subs = await _repo.GetExpiredActiveAsync(nowUtc, ct);
        return subs.Select(x => x.Id).ToList();
    }
}