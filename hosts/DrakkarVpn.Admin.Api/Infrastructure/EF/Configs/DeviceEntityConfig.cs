using DrakkarVpn.Users.Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Users.Infrastructure.EF.Configurations;

public sealed class DeviceEntityConfig : IEntityTypeConfiguration<DeviceReadEntity>
{
    public void Configure(EntityTypeBuilder<DeviceReadEntity> b)
    {
        b.ToTable("devices",  schema: "users");

        b.HasKey(x => x.DeviceId);

        b.Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(64);

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(128);

        b.Property(x => x.Platform)
            .HasColumnName("platform")
            .HasMaxLength(32);

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.LastSeenUtc)
            .HasColumnName("last_seen");

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.StatusUpdatedAtUtc)
            .HasColumnName("status_updated_at_utc");

        // ---- indexes ----

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_devices_user_id");

        b.HasIndex(x => new { x.UserId, x.CreatedAtUtc })
            .HasDatabaseName("ix_devices_user_created_at");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_devices_status");

        // FK (опционально, но норм)
        b.HasOne<UserReadEntity>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}