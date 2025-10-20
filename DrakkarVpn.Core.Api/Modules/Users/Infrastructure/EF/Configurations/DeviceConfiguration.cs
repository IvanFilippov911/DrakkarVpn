using DrakkarVpn.Core.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.EF.Configurations;

public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> b)
    {
        b.ToTable("devices");
        b.HasKey(x => x.DeviceId);

        b.Property(x => x.DeviceId)
            .HasMaxLength(64)
            .HasColumnName("device_id");

        b.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        b.Property(x => x.Name)
            .HasMaxLength(128)
            .HasColumnName("name");

        b.Property(x => x.Platform)
            .HasMaxLength(32)
            .HasColumnName("platform");

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        b.Property(x => x.LastSeen)
            .HasColumnName("last_seen");

        b.Property(x => x.Status)
            .HasConversion<short>()
            .IsRequired()
            .HasColumnName("status");

        b.HasIndex(x => x.UserId).HasDatabaseName("ix_devices_user");
    }
}