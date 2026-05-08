using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Servers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> b)
    {
        b.ToTable("servers", "servers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("name");

        b.Property(x => x.Region)
            .HasConversion(r => r.Code, v => new Region(v))
            .IsRequired()
            .HasColumnName("region");

        b.Property(x => x.PublicHost)
            .HasConversion(h => h.Value, v => new PublicHost(v))
            .IsRequired()
            .HasColumnName("public_host");

        b.Property(x => x.PublicPort)
            .IsRequired()
            .HasColumnName("public_port");

        b.Property(x => x.AgentBaseUrl)
            .HasConversion(u => u.ToString(), s => new Uri(s))
            .IsRequired()
            .HasColumnName("agent_base_url");

        b.Property(x => x.AgentTokenEncrypted)
            .IsRequired()
            .HasColumnName("agent_token_encrypted");

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("status");

        b.Property(x => x.MaxPeers)
            .HasColumnName("max_peers");

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.DesiredTransportActivationId)
            .HasColumnName("desired_transport_activation_id");

        b.Property(x => x.DesiredTransportVersion)
            .HasColumnName("desired_transport_version")
            .IsRequired();

        b.Property(x => x.AppliedTransportActivationId)
            .HasColumnName("applied_transport_activation_id");

        b.Property(x => x.AppliedTransportVersion)
            .HasColumnName("applied_transport_version")
            .IsRequired();

        b.Property(x => x.AppliedTransportAtUtc)
            .HasColumnName("applied_transport_at_utc");

        b.OwnsOne(x => x.Health, hb =>
        {
            hb.WithOwner();
            hb.Property(h => h.Reachable).HasColumnName("health_reachable").IsRequired();
            hb.Property(h => h.PeersActive).HasColumnName("health_peers_active").IsRequired();
            hb.Property(h => h.LastHealthAtUtc).HasColumnName("health_updated_at").IsRequired();
            hb.HasIndex(h => h.Reachable).HasDatabaseName("ix_servers_health_reachable");
        });

        b.OwnsOne(x => x.Metrics, mb =>
        {
            mb.WithOwner();
            mb.Property(m => m.TrafficRxBytes).HasColumnName("metrics_rx_bytes");
            mb.Property(m => m.TrafficTxBytes).HasColumnName("metrics_tx_bytes");
            mb.Property(m => m.InfraLatencyMs).HasColumnName("metrics_infra_latency_ms");
            mb.Property(m => m.VpnSpeedMbps).HasColumnName("metrics_vpn_speed_mbps");
            mb.Property(m => m.LastMetricsAtUtc).HasColumnName("metrics_updated_at").IsRequired();
        });

        b.OwnsOne(x => x.Benchmark, bb =>
        {
            bb.WithOwner();
            bb.Property(bm => bm.MaxSpeedMbps).HasColumnName("benchmark_max_speed_mbps").IsRequired();
            bb.Property(bm => bm.MeasuredAt).HasColumnName("benchmark_measured_at").IsRequired();
        });

        b.HasIndex(x => new { x.Region, x.Status })
            .HasDatabaseName("ix_servers_region_status");
        
        b.HasMany(x => x.TransportActivations)
            .WithOne()
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}