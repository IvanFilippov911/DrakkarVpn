using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF.Configurations;

public sealed class PeerProvisionJobConfig : IEntityTypeConfiguration<PeerProvisionJob>
{
    public void Configure(EntityTypeBuilder<PeerProvisionJob> b)
    {
        b.ToTable("peer_provision_jobs");
        b.HasKey(x => x.JobId);

        b.Property(x => x.JobId).HasColumnName("JobId").ValueGeneratedNever();

        b.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        b.Property(x => x.DeviceId).HasColumnName("DeviceId").HasMaxLength(128).IsRequired();
        b.Property(x => x.ServerId).HasColumnName("ServerId").IsRequired();
        
        b.Property(x => x.AgentAppliedAtUtc)
            .IsRequired(false);

        b.Property(x => x.State).HasColumnName("State").HasConversion<short>().IsRequired();
        b.Property(x => x.Attempt).HasColumnName("Attempt").IsRequired();
        b.Property(x => x.MaxAttempts).HasColumnName("MaxAttempts").IsRequired();

        b.Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc").IsRequired();
        b.Property(x => x.UpdatedAtUtc).HasColumnName("UpdatedAtUtc").IsRequired();
        b.Property(x => x.NextAttemptAtUtc).HasColumnName("NextAttemptAtUtc").IsRequired();

        b.Property(x => x.LeaseOwner).HasColumnName("LeaseOwner").HasMaxLength(128);
        b.Property(x => x.LeaseUntilUtc).HasColumnName("LeaseUntilUtc");

        b.Property(x => x.PeerId).HasColumnName("PeerId");
        b.Property(x => x.AgentPeerUuid).HasColumnName("AgentPeerUuid");

        b.Property(x => x.LastErrorCode).HasColumnName("LastErrorCode").HasMaxLength(128);
        b.Property(x => x.LastErrorMessage).HasColumnName("LastErrorMessage").HasMaxLength(2048);
        
        b.HasIndex(x => x.DeviceId)
            .IsUnique()
            .HasDatabaseName("ux_peer_provision_jobs_device_active")
            .HasFilter($@"""State"" <> {(short)PeerProvisionState.Failed}");

        b.HasIndex(x => new { x.State, x.NextAttemptAtUtc })
            .HasDatabaseName("ix_peer_provision_jobs_state_next");
        
        b.HasIndex(x => new { x.PeerId, x.AgentAppliedAtUtc });
    }
}