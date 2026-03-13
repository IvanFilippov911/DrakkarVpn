using DrakkarVpn.Users.Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.EF.Configurations;

public class UserRealtimeStatsConfig : IEntityTypeConfiguration<UserRealtimeStats>
{
    public void Configure(EntityTypeBuilder<UserRealtimeStats> b)
    {
        b.ToTable("user_realtime_stats");

        b.HasKey(x => x.UserId);

        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.SubscriptionEndUtc).HasColumnName("subscription_end_utc");
        b.Property(x => x.IsSubscriptionActive).HasColumnName("is_subscription_active");
        b.Property(x => x.SubscriptionMaxDevices).HasColumnName("subscription_max_devices");

        b.Property(x => x.DeviceCount).HasColumnName("device_count");
        b.Property(x => x.LastSeenUtc).HasColumnName("last_seen_utc");
        b.Property(x => x.IsOnline).HasColumnName("is_online");

        b.Property(x => x.Traffic24hBytes).HasColumnName("traffic_24h_bytes");

        b.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
        b.Property(x => x.LastSubscriptionStatus)
            .HasColumnName("last_subscription_status");

        
        b.HasIndex(x => x.Traffic24hBytes)
            .HasDatabaseName("idx_userstats_traffic24h");
        
        b.HasIndex(x => x.LastSeenUtc)
            .HasDatabaseName("idx_userstats_lastseen");
        
        b.HasIndex(x => x.SubscriptionEndUtc)
            .HasDatabaseName("idx_userstats_sub_end");
        
        b.HasIndex(x => x.IsSubscriptionActive)
            .HasDatabaseName("idx_userstats_sub_active");
        
        b.HasIndex(x => x.IsOnline)
            .HasDatabaseName("idx_userstats_is_online");
    }
}