using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure;

public sealed class SubscriptionDbContext : DbContext
{
    public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options) {}

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("subscriptions");
        b.ApplyConfigurationsFromAssembly(typeof(SubscriptionDbContext).Assembly);
    }
}