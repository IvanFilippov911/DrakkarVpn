using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Shared.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SubscriptionDbContext _db;

    public SubscriptionRepository(SubscriptionDbContext db) => _db = db;

    public async Task<Dictionary<Guid, ActiveSubscriptionRow>> GetActiveByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        var rows = await _db.Subscriptions
            .AsNoTracking()
            .Where(s => ids.Contains(s.UserId))
            .Where(s => s.Status == SubscriptionStatus.Active)
            .Where(s => s.EndAt > nowUtc)
            .Select(s => new ActiveSubscriptionRow(s.Id, s.UserId, s.EndAt))
            .ToListAsync(ct);
        
        return rows
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.EndAt).First());
    }
    
    public Task<Subscription?> GetActiveByUserAsync(Guid userId, DateTime nowUtc, CancellationToken ct)
    {
        return _db.Subscriptions
            .FirstOrDefaultAsync(s =>
                s.UserId == userId &&
                s.Status == SubscriptionStatus.Active &&
                s.EndAt > nowUtc, ct);
    }

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default) =>
        await _db.Subscriptions.AddAsync(subscription, ct);

    public async Task<IReadOnlyList<Subscription>> GetExpiredActiveAsync(DateTime now, CancellationToken ct = default) =>
        await _db.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndAt <= now)
            .ToListAsync(ct);
    
    public Task DeleteAsync(Subscription subscription, CancellationToken ct = default)
    {
        _db.Subscriptions.Remove(subscription);
        return Task.CompletedTask;
    }
    
    public async Task<Dictionary<Guid, SubscriptionSummaryRow>> GetSummariesForUsersAsync(
        IReadOnlyCollection<Guid> userIds, CancellationToken ct)
    {
        if (userIds is null || userIds.Count == 0)
            return new();

        var now = DateTime.UtcNow;

        var rows = await _db.Subscriptions
            .AsNoTracking()
            .Where(s => userIds.Contains(s.UserId))
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndAt > now) 
            .Select(s => new SubscriptionSummaryRow(
                s.Id,
                s.UserId,
                true,             
                s.EndAt,
                s.MaxDevices
            ))
            .ToListAsync(ct);

        
        return rows.GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Last());
    }
    
    public async Task<IReadOnlyList<SubscriptionUserRow>> GetExistingByIdsAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        CancellationToken ct)
    {
        var ids = subscriptionIds.Distinct().ToArray();

        return await _db.Subscriptions
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => new SubscriptionUserRow(s.Id, s.UserId))
            .ToListAsync(ct);
    }

    public Task ExpireManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = subscriptionIds.Distinct().ToArray();

        return _db.Subscriptions
            .Where(s => ids.Contains(s.Id))
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Status, SubscriptionStatus.Expired)
                    .SetProperty(s => s.StatusUpdatedAtUtc, markerUtc),
                ct);
    }
    
    public Task CancelManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = subscriptionIds.Distinct().ToArray();

        return _db.Subscriptions
            .Where(s => ids.Contains(s.Id))
            .Where(s => s.Status != SubscriptionStatus.Cancelled)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Status, SubscriptionStatus.Cancelled)
                    .SetProperty(s => s.StatusUpdatedAtUtc, markerUtc)
                    .SetProperty(
                        s => s.EndAt,
                        s => s.EndAt > markerUtc ? markerUtc : s.EndAt
                    ),
                ct);
    }

    public async Task<IReadOnlyList<Guid>> GetByStatusMarkerAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime markerUtc,
        SubscriptionStatus expectedStatus,
        CancellationToken ct)
    {
        var ids = subscriptionIds.Distinct().ToArray();

        return await _db.Subscriptions
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Where(s => s.StatusUpdatedAtUtc == markerUtc)
            .Where(s => s.Status == expectedStatus)
            .Select(s => s.Id)
            .ToListAsync(ct);
    }
    
    public Task RenewManyAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        DateTime nowUtc,
        TimeSpan duration,
        int maxDevices,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = subscriptionIds.Distinct().ToArray();

        return _db.Subscriptions
            .Where(s => ids.Contains(s.Id))
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Status, SubscriptionStatus.Active)
                    .SetProperty(s => s.StatusUpdatedAtUtc, markerUtc)
                    .SetProperty(s => s.MaxDevices, maxDevices)
                    .SetProperty(
                        s => s.EndAt,
                        s => (s.EndAt > nowUtc ? s.EndAt : nowUtc) + duration
                    ),
                ct);
    }

    public Task CreateManyAsync(
        IReadOnlyCollection<Subscription> subs,
        CancellationToken ct)
    {
        _db.Subscriptions.AddRange(subs);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SubscriptionUserRow>> GetUsersBySubscriptionIdsAsync(
        IReadOnlyCollection<Guid> subscriptionIds,
        CancellationToken ct)
    {
        var ids = subscriptionIds.Distinct().ToArray();

        return await _db.Subscriptions
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => new SubscriptionUserRow(s.Id, s.UserId))
            .ToListAsync(ct);
    }
    
    

}