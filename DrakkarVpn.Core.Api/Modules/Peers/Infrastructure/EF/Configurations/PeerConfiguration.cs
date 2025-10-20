using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
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

        b.Property(x => x.UserId).IsRequired().HasColumnName("user_id");
        b.Property(x => x.ServerId).IsRequired().HasColumnName("server_id");

        b.Property(x => x.SubscriptionId)
            .HasConversion(id => id.Value, v => new SubscriptionId(v))
            .IsRequired()
            .HasColumnName("subscription_id");

        b.Property(x => x.AgentPeerUuid)
            .HasConversion(id => id.Value, v => new AgentPeerUuid(v))
            .IsRequired()
            .HasColumnName("agent_peer_uuid");

        b.Property(x => x.ConfigRaw).IsRequired().HasColumnName("config_raw");

        b.Property(x => x.Status).HasConversion<int>().IsRequired().HasColumnName("status");

        b.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
        
        b.Property(x => x.DeviceId).IsRequired().HasMaxLength(64).HasColumnName("device_id");
        b.Property(x => x.LastHandshakeAt).HasColumnName("last_handshake_at");

        b.HasIndex(x => x.UserId).HasDatabaseName("ix_peers_user_id");
        b.HasIndex(x => x.ServerId).HasDatabaseName("ix_peers_server_id");
        b.HasIndex(x => x.AgentPeerUuid).IsUnique().HasDatabaseName("ux_peers_agent_peer_uuid");

        b.HasIndex(x => new { x.SubscriptionId, x.Status }).HasDatabaseName("ix_peers_sub_status");

        
        b.HasIndex(x => new { x.SubscriptionId, x.DeviceId })
            .IsUnique()
            .HasDatabaseName("ux_peers_sub_device_active")
            .HasFilter("\"status\" = 0"); 

    }
}

