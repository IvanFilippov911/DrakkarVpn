using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Entities;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Configs;

public sealed class AdminServerEntityConfig : IEntityTypeConfiguration<ServerReadEntity>
{
    public void Configure(EntityTypeBuilder<ServerReadEntity> b)
    {
        b.ToTable("servers", schema: "servers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Name).HasColumnName("name").IsRequired();
        b.Property(x => x.Region).HasColumnName("region").IsRequired();
        b.Property(x => x.PublicHost).HasColumnName("public_host").IsRequired();
        b.Property(x => x.MaxPeers).HasColumnName("max_peers");

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.HealthReachable).HasColumnName("health_reachable").IsRequired();
        b.Property(x => x.HealthPeersActive).HasColumnName("health_peers_active").IsRequired();
        b.Property(x => x.HealthUpdatedAtUtc).HasColumnName("health_updated_at").IsRequired();

        b.Property(x => x.MetricsRxBytes).HasColumnName("metrics_rx_bytes");
        b.Property(x => x.MetricsTxBytes).HasColumnName("metrics_tx_bytes");
        b.Property(x => x.MetricsInfraLatencyMs).HasColumnName("metrics_infra_latency_ms");
        b.Property(x => x.MetricsVpnSpeedMbps).HasColumnName("metrics_vpn_speed_mbps");
        b.Property(x => x.MetricsUpdatedAtUtc).HasColumnName("metrics_updated_at").IsRequired();

        b.Property(x => x.BenchmarkMaxSpeedMbps).HasColumnName("benchmark_max_speed_mbps").IsRequired();
        b.Property(x => x.BenchmarkMeasuredAt).HasColumnName("benchmark_measured_at").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        b.HasIndex(x => new { x.Region, x.Status }).HasDatabaseName("ix_servers_region_status");
        b.HasIndex(x => x.HealthReachable).HasDatabaseName("ix_servers_health_reachable");
    }
}

