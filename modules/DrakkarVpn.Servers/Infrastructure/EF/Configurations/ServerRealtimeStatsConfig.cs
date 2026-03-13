using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.Configurations;

public sealed class ServerRealtimeStatsConfig : IEntityTypeConfiguration<ServerRealtimeStats>
{
    public void Configure(EntityTypeBuilder<ServerRealtimeStats> b)
    {
        b.ToTable("server_realtime_stats");

        b.HasKey(x => x.ServerId);

        b.Property(x => x.ServerId).HasColumnName("server_id");
        b.Property(x => x.OnlinePeers).HasColumnName("online_peers");
        b.Property(x => x.TrafficLast1hBytes).HasColumnName("traffic_last_1h_bytes");
        b.Property(x => x.TrafficLast24hBytes).HasColumnName("traffic_last_24h_bytes");
        b.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");
        b.Property(x => x.TrafficCalculatedAtUtc).HasColumnName("traffic_calculated_at_utc");
    }
}