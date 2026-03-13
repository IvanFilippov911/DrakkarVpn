using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Entity;
using DrakkarVpn.Users.Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;

public sealed class AdminReadDbContext : DbContext
{
    public AdminReadDbContext(DbContextOptions<AdminReadDbContext> options) : base(options) {}

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    public DbSet<PeerEntity> Peers => Set<PeerEntity>();
    public DbSet<PeerTrafficAgg> PeerTrafficAggs => Set<PeerTrafficAgg>();
    public DbSet<SubscriptionEntity> Subscriptions => Set<SubscriptionEntity>();
    public DbSet<UserRealtimeStats> UsersRealtimeStats => Set<UserRealtimeStats>();
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<ServerMetricsHistory> ServerMetricsHistories => Set<ServerMetricsHistory>();
    public DbSet<PeerMetricsHistory>  PeerMetricsHistories => Set<PeerMetricsHistory>();
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminReadDbContext).Assembly);
    }
}