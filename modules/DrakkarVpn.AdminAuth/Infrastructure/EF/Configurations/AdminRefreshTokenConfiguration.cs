using DrakkarVpn.AdminAuth.Infrastructure.EF.Entities;
using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF.Configurations;

internal sealed class AdminRefreshTokenConfiguration : IEntityTypeConfiguration<AdminRefreshToken>
{
    public void Configure(EntityTypeBuilder<AdminRefreshToken> b)
    {
        b.ToTable("admin_refresh_tokens");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.AdminUserId)
            .HasColumnName("admin_user_id")
            .IsRequired();

        b.Property(x => x.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(512)
            .IsRequired();

        b.Property(x => x.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        b.Property(x => x.RevokedAtUtc)
            .HasColumnName("revoked_at_utc");

        b.Property(x => x.ReplacedByTokenHash)
            .HasColumnName("replaced_by_token_hash")
            .HasMaxLength(512);

        b.Property(x => x.CreatedByIp)
            .HasColumnName("created_by_ip")
            .HasMaxLength(128);

        b.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(1024);

        b.HasOne<AdminIdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.AdminUserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("ux_admin_refresh_tokens_token_hash");

        b.HasIndex(x => new { x.AdminUserId, x.RevokedAtUtc, x.ExpiresAtUtc })
            .HasDatabaseName("ix_admin_refresh_tokens_admin_user_active_window");

        b.HasIndex(x => x.ExpiresAtUtc)
            .HasDatabaseName("ix_admin_refresh_tokens_expires_at_utc");
    }
}
