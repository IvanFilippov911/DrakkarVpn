using DrakkarVpn.Servers.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class TransportIncidentAttemptConfiguration : IEntityTypeConfiguration<TransportRemediationAttempt>
{
    public void Configure(EntityTypeBuilder<TransportRemediationAttempt> b)
    {
        b.ToTable("transport_incident_attempts", "servers");

        b.HasKey(x => x.Id);

        b.Property(x => x.IncidentId).IsRequired();
        b.Property(x => x.ProfileId).IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.FailureReason)
            .HasMaxLength(TransportRemediationAttempt.FailureReasonMaxLength);

        b.Property(x => x.StartedAtUtc).IsRequired();

        b.HasIndex(x => x.IncidentId);
        b.HasIndex(x => new { x.IncidentId, x.ProfileId }).IsUnique();
    }
}