using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Infrastructure.EF.Configurations;

public sealed class ProbeNodeConfiguration : IEntityTypeConfiguration<ProbeNode>
{
    public void Configure(EntityTypeBuilder<ProbeNode> b)
    {
        b.ToTable("probe_nodes", "servers");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(ProbeNode.NameMaxLength)
            .IsRequired();

        b.Property(x => x.Region)
            .HasMaxLength(ProbeNode.RegionMaxLength)
            .IsRequired();

        b.Property(x => x.Host)
            .HasMaxLength(ProbeNode.HostMaxLength)
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.IsEnabled)
            .IsRequired();

        b.Property(x => x.LastSeenAtUtc);

        b.Property(x => x.CreatedAtUtc)
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        b.HasIndex(x => x.Name)
            .IsUnique();

        b.HasIndex(x => x.Host)
            .IsUnique();

        b.HasIndex(x => x.Status);

        b.HasIndex(x => new { x.IsEnabled, x.Status });
    }
}