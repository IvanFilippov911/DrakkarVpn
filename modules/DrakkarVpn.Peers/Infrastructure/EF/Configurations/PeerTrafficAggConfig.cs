using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF.Configurations;

public sealed class PeerTrafficAggConfig : IEntityTypeConfiguration<PeerTrafficAgg>
{
    public void Configure(EntityTypeBuilder<PeerTrafficAgg> b)
    {
        b.ToTable("peer_traffic_agg");

        b.HasKey(x => x.PeerId);

        b.Property(x => x.Last1hBytes)
            .IsRequired();

        b.Property(x => x.Last24hBytes)
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        b.HasOne(x => x.Peer)
            .WithOne()               
            .HasForeignKey<PeerTrafficAgg>(x => x.PeerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UpdatedAtUtc);
        b.HasIndex(x => x.Last24hBytes);
        b.HasIndex(x => x.Last1hBytes);
    }
}