using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}