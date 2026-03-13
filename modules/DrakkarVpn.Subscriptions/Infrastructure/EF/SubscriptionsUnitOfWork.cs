using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure;

namespace DrakkarVpn.Subscriptions.Infrastructure.EF;

public sealed class SubscriptionsUnitOfWork : ISubscriptionsUnitOfWork
{
    private readonly SubscriptionDbContext _db;

    public SubscriptionsUnitOfWork(SubscriptionDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}