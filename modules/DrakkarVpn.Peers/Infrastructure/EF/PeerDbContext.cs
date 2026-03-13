using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;

public sealed class PeerDbContext : DbContext
{
    public PeerDbContext(DbContextOptions<PeerDbContext> options) : base(options) { }

    public DbSet<Peer> Peers => Set<Peer>();
    public DbSet<PeerMetricsHistory> PeerMetricsHistory => Set<PeerMetricsHistory>();
    public DbSet<PeerProvisionJob> PeerProvisionJobs => Set<PeerProvisionJob>();
    public DbSet<PeerSyncIssueEntity> PeerSyncIssues => Set<PeerSyncIssueEntity>();
    public DbSet<PeerTrafficAgg> PeerTrafficAggs => Set<PeerTrafficAgg>();
    public DbSet<DeviceReadRow> Devices => Set<DeviceReadRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("peers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PeerDbContext).Assembly);

        modelBuilder.Entity<DeviceReadRow>(e =>
        {
            e.ToTable("devices", "users", t => t.ExcludeFromMigrations());
            e.HasNoKey();

            e.Property(x => x.DeviceId).HasColumnName("device_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Status).HasColumnName("status");
        });

        base.OnModelCreating(modelBuilder);
    }
}