using DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Admin.Api.Infrastructure.EF.Configs;

public sealed class AdminPeerTrafficAggReadEntityConfig : IEntityTypeConfiguration<AdminPeerTrafficAggReadEntity>
{
    public void Configure(EntityTypeBuilder<AdminPeerTrafficAggReadEntity> b)
    {
        b.ToTable("peer_traffic_agg", schema: "peers");

        b.HasKey(x => x.PeerId);

        b.Property(x => x.PeerId)
            .HasColumnName("PeerId")
            .IsRequired();

        b.Property(x => x.Last24hBytes)
            .HasColumnName("Last24hBytes")
            .IsRequired();
    }
}

