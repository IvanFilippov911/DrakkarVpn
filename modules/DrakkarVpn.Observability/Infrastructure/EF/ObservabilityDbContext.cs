using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Observability.Infrastructure.EF;

public sealed class ObservabilityDbContext : DbContext
{
    public ObservabilityDbContext(DbContextOptions<ObservabilityDbContext> options)
        : base(options)
    {
    }

    public DbSet<CoreAlert> CoreAlerts => Set<CoreAlert>();
    public DbSet<CoreErrorEvent> CoreErrorEvents => Set<CoreErrorEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("observability");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ObservabilityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}