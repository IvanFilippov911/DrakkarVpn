using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Entity;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Entities;
using DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using DrakkarVpn.Users.Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;

public sealed class AdminReadDbContext : DbContext
{
    public AdminReadDbContext(DbContextOptions<AdminReadDbContext> options) : base(options) {}

    public DbSet<UserReadEntity> Users => Set<UserReadEntity>();
    public DbSet<DeviceReadEntity> Devices => Set<DeviceReadEntity>();
    public DbSet<PeerReadEntity> Peers => Set<PeerReadEntity>();
    public DbSet<SubscriptionReadEntity> Subscriptions => Set<SubscriptionReadEntity>();
    public DbSet<ServerReadEntity> Servers => Set<ServerReadEntity>();
    public DbSet<AdminPeerTrafficAggReadEntity> AdminPeerTrafficAggs => Set<AdminPeerTrafficAggReadEntity>();
    public DbSet<AdminUserRealtimeStatsReadEntity> AdminUsersRealtimeStats => Set<AdminUserRealtimeStatsReadEntity>();
    public DbSet<AdminServerMetricsHistoryReadEntity> AdminServerMetricsHistories => Set<AdminServerMetricsHistoryReadEntity>();
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminReadDbContext).Assembly);
    }
}