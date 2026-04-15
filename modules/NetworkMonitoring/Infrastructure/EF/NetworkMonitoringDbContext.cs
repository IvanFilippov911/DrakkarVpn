using Microsoft.EntityFrameworkCore;
using NetworkMonitoring.Domain;
using NetworkMonitoring.Infrastructure.EF.Configurations;
using NetworkMonitoring.Infrastructure.EF.Entities;

namespace NetworkMonitoring.Infrastructure.EF;

public sealed class NetworkMonitoringDbContext : DbContext
{
    public NetworkMonitoringDbContext(DbContextOptions<NetworkMonitoringDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProbeNode> ProbeNodes => Set<ProbeNode>();
    public DbSet<ProbeResult> ProbeResults => Set<ProbeResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProbeNodeConfiguration());
        modelBuilder.ApplyConfiguration(new ProbeResultConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}