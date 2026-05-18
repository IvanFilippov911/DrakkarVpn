using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums.TransportProfile;
using DrakkarVpn.Servers.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class ServerTransportApplyJobConfiguration : IEntityTypeConfiguration<ServerTransportApplyJob>
{
    public void Configure(EntityTypeBuilder<ServerTransportApplyJob> b)
    {
        b.ToTable("server_transport_apply_jobs", "servers");

        b.HasKey(x => x.JobId);

        b.Property(x => x.JobId)
            .HasColumnName("job_id")
            .ValueGeneratedNever();

        b.Property(x => x.ServerId)
            .HasColumnName("server_id")
            .IsRequired();

        b.Property(x => x.ActivationId)
            .HasColumnName("activation_id")
            .IsRequired();

        b.Property(x => x.TargetTransportVersion)
            .HasColumnName("target_transport_version")
            .IsRequired();

        b.Property(x => x.State)
            .HasColumnName("state")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.LeaseUntilUtc)
            .HasColumnName("lease_until_utc")
            .HasColumnType("timestamptz");

        b.Property(x => x.LeaseOwner)
            .HasColumnName("lease_owner")
            .HasMaxLength(128);

        b.Property(x => x.Attempt)
            .HasColumnName("attempt")
            .IsRequired();

        b.Property(x => x.MaxAttempt)
            .HasColumnName("max_attempt")
            .IsRequired();

        b.Property(x => x.NextAttemptUtc)
            .HasColumnName("next_attempt_utc")
            .HasColumnType("timestamptz")
            .IsRequired();

        b.Property(x => x.LastErrorCode)
            .HasColumnName("last_error_code")
            .HasMaxLength(128);

        b.Property(x => x.LastErrorMessage)
            .HasColumnName("last_error_message")
            .HasMaxLength(2048);

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz")
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz")
            .IsRequired();

        b.Property(x => x.CompletedAtUtc)
            .HasColumnName("completed_at_utc")
            .HasColumnType("timestamptz");

        b.HasIndex(x => new { x.State, x.NextAttemptUtc })
            .HasDatabaseName("ix_server_transport_apply_jobs_state_next_attempt");

        b.HasIndex(x => x.ServerId)
            .HasDatabaseName("ix_server_transport_apply_jobs_server_id");

        b.HasIndex(x => x.ServerId)
            .IsUnique()
            .HasFilter(
                $"\"state\" IN ({(int)ServerTransportApplyJobStatus.Pending}, {(int)ServerTransportApplyJobStatus.Processing})")
            .HasDatabaseName("ux_server_transport_apply_jobs_one_in_flight_per_server");

        b.HasIndex(x => new { x.ServerId, x.TargetTransportVersion })
            .IsUnique()
            .HasDatabaseName("ux_server_transport_apply_jobs_server_target_transport_version");

        b.HasIndex(x => x.LeaseUntilUtc)
            .HasDatabaseName("ix_server_transport_apply_jobs_lease_until");

        b.HasOne<Server>()
            .WithMany()
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.ToTable(t =>
        {
            t.HasCheckConstraint(
                "ck_server_transport_apply_jobs_attempt_nonneg",
                "\"attempt\" >= 0");

            t.HasCheckConstraint(
                "ck_server_transport_apply_jobs_max_attempt_pos",
                "\"max_attempt\" > 0");

            t.HasCheckConstraint(
                "ck_server_transport_apply_jobs_terminal_completed_at",
                $"(\"state\" <> {(int)ServerTransportApplyJobStatus.Completed}) OR (\"completed_at_utc\" IS NOT NULL)");
        });
    }
}
