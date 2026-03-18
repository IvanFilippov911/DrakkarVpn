using DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Admin.Api.Infrastructure.EF.Configs;

public sealed class AdminServerMetricsHistoryReadEntityConfig : IEntityTypeConfiguration<AdminServerMetricsHistoryReadEntity>
{
    public void Configure(EntityTypeBuilder<AdminServerMetricsHistoryReadEntity> b)
    {
        b.ToTable("server_metrics_history", schema: "servers");

        b.HasKey(x => new { x.PeriodStartUtc, x.ServerId });

        b.Property(x => x.PeriodStartUtc)
            .HasColumnName("period_start")
            .HasColumnType("timestamptz");

        b.Property(x => x.ServerId)
            .HasColumnName("server_id");

        b.Property(x => x.TrafficRxDeltaBytes)
            .HasColumnName("traffic_rx_delta_bytes");

        b.Property(x => x.TrafficTxDeltaBytes)
            .HasColumnName("traffic_tx_delta_bytes");

        b.Property(x => x.VpnSpeedMbps)
            .HasColumnName("vpn_speed_mbps")
            .HasPrecision(10, 2);

        b.Property(x => x.InfraLatencyMs)
            .HasColumnName("infra_latency_ms")
            .HasPrecision(10, 2);
    }
}

