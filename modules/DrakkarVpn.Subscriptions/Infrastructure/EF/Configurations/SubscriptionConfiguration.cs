using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> b)
    {
        b.ToTable("subscriptions");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.Property(x => x.StartAt).HasColumnName("start_at").IsRequired();
        b.Property(x => x.EndAt).HasColumnName("end_at").IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.MaxDevices)
            .IsRequired()
            .HasDefaultValue(1)
            .HasColumnName("max_devices");
        
        b.Property(x => x.StatusUpdatedAtUtc)
            .HasColumnName("status_updated_at_utc")
            .IsRequired();
        
        b.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("\"status\" = 1")
            .HasDatabaseName("ux_subscriptions_user_active");
        
        b.HasIndex(x => new { x.UserId, x.Status, x.EndAt })
            .HasDatabaseName("ix_subscriptions_user_status_endat");
        
        b.HasIndex(x => new { x.UserId, x.Status, x.EndAt })
            .IsDescending(false, false, true)
            .HasDatabaseName("ix_subscriptions_user_status_endat_desc");
        
        b.HasIndex(x => new { x.Status, x.StatusUpdatedAtUtc, x.Id })
            .HasDatabaseName("ix_subs_status_statusupdatedat_id");
    }
}