using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
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
        
        b.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Subscription>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        b.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("\"status\" = 1")
            .HasDatabaseName("ux_subscriptions_user_active");
        
        b.HasIndex(x => new { x.UserId, x.Status, x.EndAt })
            .HasDatabaseName("ix_subscriptions_user_status_endat");
    }
}