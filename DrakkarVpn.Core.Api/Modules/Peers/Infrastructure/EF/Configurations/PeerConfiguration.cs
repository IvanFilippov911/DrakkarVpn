using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF.Configurations;

public sealed class PeerConfiguration : IEntityTypeConfiguration<Peer>
{
    public void Configure(EntityTypeBuilder<Peer> b)
    {
        b.ToTable("peers");
        
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id");
        
        b.Property(x => x.ServerId)
            .IsRequired()
            .HasColumnName("server_id");

        b.Property(x => x.AgentPeerUuid)
            .IsRequired()
            .HasColumnName("agent_peer_uuid");

        b.Property(x => x.ConfigRaw)
            .IsRequired()
            .HasColumnName("config_raw");

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("status");

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        b.Property(x => x.DeviceId)
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnName("device_id");
        
        b.Property(x => x.LastDataAt)
            .HasColumnName("last_data_at");

        b.Property(x => x.TotalRxBytes)
            .IsRequired()
            .HasColumnName("total_rx_bytes")
            .HasColumnType("bigint");

        b.Property(x => x.TotalTxBytes)
            .IsRequired()
            .HasColumnName("total_tx_bytes")
            .HasColumnType("bigint");

        b.Property(x => x.VpnLatencyMs)
            .HasColumnName("vpn_latency_ms"); 

        b.Property(x => x.LastLatencyAt)
            .HasColumnName("last_latency_at");

        b.Property(x => x.LastPolledAt)
            .HasColumnName("last_polled_at");
        
        b.Property(x => x.IsOnline)
            .IsRequired()
            .HasColumnName("is_online")
            .HasDefaultValue(false);
        
        b.Property(x => x.SpeedMbps)
            .HasColumnName("speed_mbps")
            .HasPrecision(10, 2);
        
        b.HasOne<Server>()
            .WithMany()
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<Device>()
            .WithOne()
            .HasForeignKey<Peer>(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        b.HasIndex(x => x.ServerId)
            .HasDatabaseName("ix_peers_server_id");

        b.HasIndex(x => x.AgentPeerUuid)
            .IsUnique()
            .HasDatabaseName("ux_peers_agent_peer_uuid");
        
        b.HasIndex(x => new { x.ServerId, x.Status })
            .HasDatabaseName("ix_peers_server_status");
        
        b.HasIndex(x => x.DeviceId)
            .IsUnique()
            .HasFilter("\"status\" = 0")
            .HasDatabaseName("ux_peers_device_active");
        
        
        b.ToTable(t =>
        {
            t.HasCheckConstraint("ck_peers_total_rx_bytes_nonneg", "total_rx_bytes >= 0");
            t.HasCheckConstraint("ck_peers_total_tx_bytes_nonneg", "total_tx_bytes >= 0");
            t.HasCheckConstraint("ck_peers_vpn_latency_ms_valid", "vpn_latency_ms IS NULL OR vpn_latency_ms >= 0");
        });
    }
}