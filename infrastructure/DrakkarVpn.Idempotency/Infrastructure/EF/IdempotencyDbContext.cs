using DrakkarVpn.Core.Api.Modules.Idempotency.Domain;
using Microsoft.EntityFrameworkCore;

namespace Idempotency.Infrastructure.EF;

public sealed class IdempotencyDbContext : DbContext
{
    public IdempotencyDbContext(DbContextOptions<IdempotencyDbContext> options)
        : base(options)
    {
    }

    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("idempotency");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdempotencyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}