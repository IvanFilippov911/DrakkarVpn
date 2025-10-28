using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Shared.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _db;

    public SubscriptionRepository(AppDbContext db) => _db = db;

    public async Task<Subscription?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default) =>
        await _db.Subscriptions
            .Where(x => x.UserId == userId && x.Status == SubscriptionStatus.Active && x.EndAt > DateTime.UtcNow)
            .FirstOrDefaultAsync(ct);

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

}