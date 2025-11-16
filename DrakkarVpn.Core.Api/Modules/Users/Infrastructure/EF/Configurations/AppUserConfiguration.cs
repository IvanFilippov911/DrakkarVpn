using DrakkarVpn.Core.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.EF.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.ToTable("app_users");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.TelegramId)
            .HasColumnName("telegram_id")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.Username)             
            .HasColumnName("username")
            .HasMaxLength(64);   

        b.Property(x => x.IsInternal)
            .HasColumnName("is_internal")
            .IsRequired();            

        b.Property(x => x.BanReason)
            .HasColumnName("ban_reason")
            .HasMaxLength(512);      

        b.Property(x => x.BannedAtUtc)
            .HasColumnName("banned_at_utc"); 

        b.HasIndex(x => x.TelegramId)
            .IsUnique()
            .HasDatabaseName("ux_users_telegram_id");

        b.HasIndex(x => new { x.Status, x.CreatedAt, x.Id })
            .IsDescending(false, true, true)
            .HasDatabaseName("ix_users_status_createdat_desc_id_desc");

        b.HasIndex(x => new { x.CreatedAt, x.Id })
            .IsDescending(true, true)
            .HasDatabaseName("ix_users_createdat_desc_id_desc");

        b.HasIndex(x => x.IsInternal)
            .HasDatabaseName("ix_users_is_internal");
    }
}