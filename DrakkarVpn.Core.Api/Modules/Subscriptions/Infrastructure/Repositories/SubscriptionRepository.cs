using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
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

    public async Task<Subscription?> GetByIdAsync(SubscriptionId id, CancellationToken ct = default) =>
        await _db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default) =>
        await _db.Subscriptions.AddAsync(subscription, ct);

    public async Task<IReadOnlyList<Subscription>> GetExpiredActiveAsync(DateTime now, CancellationToken ct = default) =>
        await _db.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndAt <= now)
            .ToListAsync(ct);

}