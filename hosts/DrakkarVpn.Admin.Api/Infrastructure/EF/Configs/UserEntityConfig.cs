using DrakkarVpn.Users.Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Users.Infrastructure.EF.Configurations;

public sealed class UserEntityConfig : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> b)
    {
        b.ToTable("users");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.TelegramId)
            .HasColumnName("telegram_id")
            .IsRequired();

        b.Property(x => x.Username)
            .HasColumnName("username")
            .HasMaxLength(64);

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        b.Property(x => x.IsInternal)
            .HasColumnName("is_internal")
            .IsRequired();

        b.Property(x => x.BanReason)
            .HasColumnName("ban_reason")
            .HasMaxLength(256);

        b.Property(x => x.BannedAtUtc)
            .HasColumnName("banned_at_utc");

        b.Property(x => x.ModerationUpdatedAtUtc)
            .HasColumnName("moderation_updated_at_utc");

        // ---- indexes ----

        b.HasIndex(x => x.TelegramId)
            .IsUnique()
            .HasDatabaseName("ux_users_telegram_id");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_users_status");

        b.HasIndex(x => x.IsInternal)
            .HasDatabaseName("ix_users_is_internal");

        b.HasIndex(x => x.ModerationUpdatedAtUtc)
            .HasDatabaseName("ix_users_moderation_updated_at");
    }
}