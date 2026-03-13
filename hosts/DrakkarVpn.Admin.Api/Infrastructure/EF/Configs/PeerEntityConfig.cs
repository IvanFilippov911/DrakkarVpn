using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF.Configurations;

public sealed class PeerEntityConfig : IEntityTypeConfiguration<PeerEntity>
{
    public void Configure(EntityTypeBuilder<PeerEntity> b)
    {
        b.ToTable("peers");
        
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id");
        
        b.Property(x => x.ServerId)
            .HasColumnName("server_id")
            .IsRequired();

        b.Property(x => x.AgentPeerUuid)
            .HasColumnName("agent_peer_uuid")
            .IsRequired();

        b.Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(64)
            .IsRequired();
        
        b.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();
        
        b.Property(x => x.IsOnline)
            .HasColumnName("is_online")
            .IsRequired();
        
        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.StatusUpdatedAtUtc)
            .HasColumnName("status_updated_at_utc")
            .IsRequired();
        
        b.HasIndex(x => x.ServerId)
            .HasDatabaseName("ix_peers_server_id");

        b.HasIndex(x => x.DeviceId)
            .HasDatabaseName("ix_peers_device_id");

        b.HasIndex(x => x.IsOnline)
            .HasDatabaseName("ix_peers_is_online");

        b.HasIndex(x => new { x.ServerId, x.IsOnline })
            .HasDatabaseName("ix_peers_server_online");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_peers_status");
    }
}