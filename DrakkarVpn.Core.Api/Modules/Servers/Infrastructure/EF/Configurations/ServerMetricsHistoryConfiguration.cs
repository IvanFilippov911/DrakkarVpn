using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.Configurations;

public sealed class ServerMetricsHistoryConfiguration : IEntityTypeConfiguration<ServerMetricsHistory>
{
    public void Configure(EntityTypeBuilder<ServerMetricsHistory> b)
    {
        b.ToTable("server_metrics_history");
        b.HasKey(x => new { x.PeriodStartUtc, x.ServerId });

        b.Property(x => x.PeriodStartUtc).HasColumnName("period_start").HasColumnType("timestamptz");
        b.Property(x => x.ServerId).HasColumnName("server_id");
        b.Property(x => x.Reachable).HasColumnName("reachable");
        b.Property(x => x.PeersActive).HasColumnName("peers_active");
        b.Property(x => x.MaxPeers).HasColumnName("max_peers");
        b.Property(x => x.TrafficRxBytes).HasColumnName("traffic_rx_bytes");
        b.Property(x => x.TrafficTxBytes).HasColumnName("traffic_tx_bytes");
        b.Property(x => x.VpnSpeedMbps).HasColumnName("vpn_speed_mbps").HasPrecision(10,2);
        b.Property(x => x.InfraLatencyMs).HasColumnName("infra_latency_ms").HasPrecision(10,2);
    }
}
