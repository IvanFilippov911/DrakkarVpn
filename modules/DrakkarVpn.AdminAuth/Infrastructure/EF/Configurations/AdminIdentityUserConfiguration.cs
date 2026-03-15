using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF.Configurations;

internal sealed class AdminIdentityUserConfiguration : IEntityTypeConfiguration<AdminIdentityUser>
{
    public void Configure(EntityTypeBuilder<AdminIdentityUser> b)
    {
        b.ToTable("admin_users");

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.UserName)
            .HasColumnName("user_name");

        b.Property(x => x.NormalizedUserName)
            .HasColumnName("normalized_user_name");

        b.Property(x => x.Email)
            .HasColumnName("email");

        b.Property(x => x.NormalizedEmail)
            .HasColumnName("normalized_email");

        b.Property(x => x.EmailConfirmed)
            .HasColumnName("email_confirmed");

        b.Property(x => x.PasswordHash)
            .HasColumnName("password_hash");

        b.Property(x => x.SecurityStamp)
            .HasColumnName("security_stamp");

        b.Property(x => x.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");

        b.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number");

        b.Property(x => x.PhoneNumberConfirmed)
            .HasColumnName("phone_number_confirmed");

        b.Property(x => x.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled");

        b.Property(x => x.LockoutEnd)
            .HasColumnName("lockout_end");

        b.Property(x => x.LockoutEnabled)
            .HasColumnName("lockout_enabled");

        b.Property(x => x.AccessFailedCount)
            .HasColumnName("access_failed_count");

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        b.Property(x => x.LastLoginAtUtc)
            .HasColumnName("last_login_at_utc");

        b.HasIndex(x => x.Email)
            .HasDatabaseName("ix_admin_users_email");

        b.HasIndex(x => x.IsActive)
            .HasDatabaseName("ix_admin_users_is_active");
    }
}
