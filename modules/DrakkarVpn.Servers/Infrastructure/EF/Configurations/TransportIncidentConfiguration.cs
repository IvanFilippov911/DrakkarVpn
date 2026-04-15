using DrakkarVpn.Servers.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class TransportIncidentConfiguration : IEntityTypeConfiguration<TransportIncident>
{
    public void Configure(EntityTypeBuilder<TransportIncident> b)
    {
        b.ToTable("transport_incidents", "servers");

        b.HasKey(x => x.Id);

        b.Property(x => x.ServerId).IsRequired();
        b.Property(x => x.ActiveProfileId).IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Scope)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Reason)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.TriggeredBy)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.OpenedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc).IsRequired();

        b.Property(x => x.LastError)
            .HasMaxLength(TransportIncident.LastErrorMaxLength);

        b.Property(x => x.Notes)
            .HasMaxLength(TransportIncident.NotesMaxLength);

        b.Property(x => x.AffectedProbeCount).IsRequired();
        b.Property(x => x.FailedProbeCount).IsRequired();

        b.HasMany(x => x.Attempts)
            .WithOne()
            .HasForeignKey(x => x.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(x => x.Attempts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasIndex(x => x.ServerId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => new { x.ServerId, x.Status });
        
        b.HasIndex(x => new { x.ServerId, x.Status })
         .HasFilter("\"Status\" IN (1,2,3)")
         .IsUnique();
    }
}