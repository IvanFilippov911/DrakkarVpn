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

        b.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        b.Property(x => x.ServerId)
            .IsRequired()
            .HasColumnName("server_id");
        
        b.Property(x => x.SubscriptionId)
            .HasConversion(id => id.Value, v => new SubscriptionId(v))
            .IsRequired();

        b.Property(x => x.AgentPeerUuid)
            .HasConversion(id => id.Value, v => new AgentPeerUuid(v))
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
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at");
        
        b.HasIndex(x => x.UserId).HasDatabaseName("ix_peers_user_id");
        b.HasIndex(x => x.ServerId).HasDatabaseName("ix_peers_server_id");
        b.HasIndex(x => x.AgentPeerUuid).IsUnique().HasDatabaseName("ux_peers_agent_peer_uuid");
    }
}