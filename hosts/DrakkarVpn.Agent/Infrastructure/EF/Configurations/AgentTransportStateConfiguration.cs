using DrakkarVpn.Agent.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Agent.Infrastructure.EF.Configurations;

public sealed class AgentTransportStateConfiguration : IEntityTypeConfiguration<AgentTransportState>
{
    public void Configure(EntityTypeBuilder<AgentTransportState> b)
    {
        b.ToTable(
            "agent_transport_state",
            t => t.HasCheckConstraint(
                "ck_agent_transport_state_singleton",
                "id = 1"));

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.ServerId)
            .HasColumnName("server_id")
            .IsRequired();

        b.Property(x => x.ActivationId)
            .HasColumnName("activation_id")
            .IsRequired();

        b.Property(x => x.OperationId)
            .HasColumnName("operation_id")
            .IsRequired();

        b.Property(x => x.PayloadHash)
            .HasColumnName("payload_hash")
            .HasMaxLength(128)
            .IsRequired();

        b.Property(x => x.AppliedAtUtc)
            .HasColumnName("applied_at_utc")
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();
    }
}
