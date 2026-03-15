using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF.Configurations;

internal static class AdminIdentityInfrastructureModelBuilderExtensions
{
    public static ModelBuilder ApplyAdminIdentityInfrastructureMappings(this ModelBuilder b)
    {
        b.Entity<IdentityUserClaim<Guid>>().ToTable("admin_user_claims");
        b.Entity<IdentityUserRole<Guid>>().ToTable("admin_user_roles");
        b.Entity<IdentityUserLogin<Guid>>().ToTable("admin_user_logins");
        b.Entity<IdentityRoleClaim<Guid>>().ToTable("admin_role_claims");
        b.Entity<IdentityUserToken<Guid>>().ToTable("admin_user_tokens");

        b.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.ClaimType).HasColumnName("claim_type");
            entity.Property(x => x.ClaimValue).HasColumnName("claim_value");
        });

        b.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");
        });

        b.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.Property(x => x.LoginProvider).HasColumnName("login_provider");
            entity.Property(x => x.ProviderKey).HasColumnName("provider_key");
            entity.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name");
            entity.Property(x => x.UserId).HasColumnName("user_id");
        });

        b.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.ClaimType).HasColumnName("claim_type");
            entity.Property(x => x.ClaimValue).HasColumnName("claim_value");
        });

        b.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.LoginProvider).HasColumnName("login_provider");
            entity.Property(x => x.Name).HasColumnName("name");
            entity.Property(x => x.Value).HasColumnName("value");
        });

        return b;
    }
}
