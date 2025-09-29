using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.Configurations;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Infrastructure.EF;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<Peer> Peers => Set<Peer>();
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}