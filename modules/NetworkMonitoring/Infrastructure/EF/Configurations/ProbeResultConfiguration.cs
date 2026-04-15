using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkMonitoring.Infrastructure.EF.Entities;

namespace NetworkMonitoring.Infrastructure.EF.Configurations;

public sealed class ProbeResultConfiguration : IEntityTypeConfiguration<ProbeResult>
{
    public void Configure(EntityTypeBuilder<ProbeResult> b)
    {
        b.ToTable("probe_results", "network_monitoring");

        b.HasKey(x => x.Id);

        b.Property(x => x.ProbeNodeId)
            .IsRequired();

        b.Property(x => x.ServerId)
            .IsRequired();

        b.Property(x => x.ProfileId)
            .IsRequired();

        b.Property(x => x.Success)
            .IsRequired();

        b.Property(x => x.LatencyMs);

        b.Property(x => x.ErrorCode)
            .HasMaxLength(ProbeResult.ErrorCodeMaxLength);

        b.Property(x => x.ErrorMessage)
            .HasMaxLength(ProbeResult.ErrorMessageMaxLength);

        b.Property(x => x.CheckedAtUtc)
            .IsRequired();

        b.HasIndex(x => x.ProbeNodeId);
        b.HasIndex(x => x.ServerId);
        b.HasIndex(x => x.ProfileId);
        b.HasIndex(x => x.CheckedAtUtc);
        b.HasIndex(x => new { x.ServerId, x.ProfileId, x.CheckedAtUtc });
        b.HasIndex(x => new { x.ProbeNodeId, x.CheckedAtUtc });
    }
}