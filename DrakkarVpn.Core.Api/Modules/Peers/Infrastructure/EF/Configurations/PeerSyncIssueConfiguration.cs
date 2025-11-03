using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF.Configurations;

public sealed class PeerSyncIssueConfiguration : IEntityTypeConfiguration<PeerSyncIssueEntity>
{
    public void Configure(EntityTypeBuilder<PeerSyncIssueEntity> b)
    {
        b.ToTable("peer_sync_issues");
        b.HasKey(x => x.Id);

        b.Property(x => x.AgentPeerUuid);
        b.Property(x => x.Type).IsRequired();
        b.Property(x => x.Details).HasMaxLength(512);

        b.HasIndex(x => new { x.ServerId, x.Type, x.DetectedAtUtc });
    }
}