using DrakkarVpn.AdminAuth.Infrastructure.EF.Configurations;
using DrakkarVpn.AdminAuth.Infrastructure.EF.Entities;
using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF;

public sealed class AdminAuthDbContext
    : IdentityDbContext<
        AdminIdentityUser,
        AdminIdentityRole,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
{
    public AdminAuthDbContext(DbContextOptions<AdminAuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdminRefreshToken> RefreshTokens => Set<AdminRefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.HasDefaultSchema("admin_auth");
        b.Entity<AdminIdentityUser>().ToTable("admin_users");
        b.Entity<AdminIdentityRole>().ToTable("admin_roles");
        b.ApplyConfigurationsFromAssembly(typeof(AdminAuthDbContext).Assembly);
        b.ApplyAdminIdentityInfrastructureMappings();
    }
}
