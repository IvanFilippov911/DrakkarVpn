using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF.Configurations;

public sealed class PeerMetricsHistoryConfiguration : IEntityTypeConfiguration<PeerMetricsHistory>
{
    public void Configure(EntityTypeBuilder<PeerMetricsHistory> b)
    {
        b.ToTable("peer_metrics_history");
        
        b.HasKey(x => new { x.PeriodStartUtc, x.ServerId, x.PeerId });

        b.Property(x => x.PeriodStartUtc)
            .HasColumnName("period_start")
            .HasColumnType("timestamptz");

        b.Property(x => x.ServerId)
            .HasColumnName("server_id");

        b.Property(x => x.PeerId)
            .HasColumnName("peer_id");

        b.Property(x => x.TotalRxBytes)
            .HasColumnName("total_rx_bytes");

        b.Property(x => x.TotalTxBytes)
            .HasColumnName("total_tx_bytes");

        b.Property(x => x.IsOnline)
            .HasColumnName("is_online");
        
        b.Property(x => x.SpeedMbps)
            .HasColumnName("speed_mbps")
            .HasPrecision(10, 2);

        b.Property(x => x.VpnLatencyMs)
            .HasColumnName("vpn_latency_ms")
            .HasColumnType("double precision");
        
        b.Property(x => x.RxDeltaBytes)
            .HasColumnName("rx_delta_bytes")
            .HasColumnType("bigint")
            .IsRequired();

        b.Property(x => x.TxDeltaBytes)
            .HasColumnName("tx_delta_bytes")
            .HasColumnType("bigint")
            .IsRequired();
        
        b.HasIndex(x => new { x.ServerId, x.PeriodStartUtc, x.IsOnline })
            .HasDatabaseName("ix_peer_hist_srv_period_online");
        
        b.HasIndex(x => new { x.ServerId, x.PeriodStartUtc, x.PeerId })
            .HasDatabaseName("ix_peer_metrics_server_period_peer");
        
        b.HasIndex(x => new { x.PeerId, x.PeriodStartUtc });
        b.HasIndex(x => x.PeriodStartUtc);
    }
}