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
            .HasConversion(id => id.Value, v => new PeerId(v))
            .HasColumnName("id");

        b.Property(x => x.ServerId).IsRequired().HasColumnName("server_id");

        b.Property(x => x.AgentPeerUuid)
            .HasConversion(id => id.Value, v => new AgentPeerUuid(v))
            .IsRequired()
            .HasColumnName("agent_peer_uuid");

        b.Property(x => x.ConfigRaw).IsRequired().HasColumnName("config_raw");

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");

        b.Property(x => x.DeviceId).IsRequired().HasMaxLength(64).HasColumnName("device_id");
        b.Property(x => x.LastHandshakeAt).HasColumnName("last_handshake_at");
        
        b.HasOne<Server>()
            .WithMany()
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        b.HasOne<Device>()
            .WithOne()
            .HasForeignKey<Peer>(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        b.HasIndex(x => x.ServerId).HasDatabaseName("ix_peers_server_id");
        b.HasIndex(x => x.AgentPeerUuid).IsUnique().HasDatabaseName("ux_peers_agent_peer_uuid");
        b.HasIndex(x => x.DeviceId)
            .IsUnique()
            .HasFilter("\"status\" = 0")           
            .HasDatabaseName("ux_peers_device_active");
    }
}

