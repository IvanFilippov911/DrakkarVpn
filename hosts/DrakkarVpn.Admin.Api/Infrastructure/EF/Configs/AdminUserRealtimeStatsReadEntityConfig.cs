using DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Admin.Api.Infrastructure.EF.Configs;

public sealed class AdminUserRealtimeStatsReadEntityConfig : IEntityTypeConfiguration<AdminUserRealtimeStatsReadEntity>
{
    public void Configure(EntityTypeBuilder<AdminUserRealtimeStatsReadEntity> b)
    {
        b.ToTable("user_realtime_stats", schema: "users");

        b.HasKey(x => x.UserId);

        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.SubscriptionEndUtc).HasColumnName("subscription_end_utc");
        b.Property(x => x.IsSubscriptionActive).HasColumnName("is_subscription_active");
        b.Property(x => x.LastSubscriptionStatus).HasColumnName("last_subscription_status");
        b.Property(x => x.SubscriptionMaxDevices).HasColumnName("subscription_max_devices");
        b.Property(x => x.DeviceCount).HasColumnName("device_count");
        b.Property(x => x.LastSeenUtc).HasColumnName("last_seen_utc");
        b.Property(x => x.IsOnline).HasColumnName("is_online");
        b.Property(x => x.Traffic24hBytes).HasColumnName("traffic_24h_bytes");
        b.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
    }
}

