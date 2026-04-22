using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using DrakkarVpn.Servers.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;

public sealed class ServerDbContext : DbContext
{
    public ServerDbContext(DbContextOptions<ServerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Server> Servers => Set<Server>();
    public DbSet<TransportProfile> ServerTransportProfiles => Set<TransportProfile>();
    public DbSet<ServerPollState> ServerPollStates => Set<ServerPollState>();
    public DbSet<ServerRealtimeStats> ServerRealtimeStats => Set<ServerRealtimeStats>();
    public DbSet<ServerMetricsHistory> ServersMetricsHistories => Set<ServerMetricsHistory>();
    public DbSet<TransportIncident> TransportIncidents => Set<TransportIncident>();
    public DbSet<TransportRemediationAttempt> TransportIncidentAttempts => Set<TransportRemediationAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("servers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
        
        modelBuilder.Entity<Server>()
            .HasOne<ServerPollState>()
            .WithOne()
            .HasForeignKey<ServerPollState>(x => x.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}