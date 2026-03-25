using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.Configurations;

internal sealed class ServerPollStateConfiguration : IEntityTypeConfiguration<ServerPollState>
{
    public void Configure(EntityTypeBuilder<ServerPollState> b)
    {
        b.ToTable("server_poll_states");

        b.HasKey(x => x.ServerId);
        
        b.Property(x => x.ServerId)
            .ValueGeneratedNever();
        
        b.Property(x => x.LeaseOwner)
            .HasMaxLength(128);

        b.Property(x => x.LeaseUntilUtc);
        b.Property(x => x.LastLeaseRenewedUtc);
        
        b.Property(x => x.ConsecutiveFailures)
            .IsRequired();

        b.Property(x => x.BackoffUntilUtc);
        b.Property(x => x.LastReachableUtc);
        
        b.Property(x => x.LastPollStartedUtc);
        b.Property(x => x.LastPollFinishedUtc);
        b.Property(x => x.LastPollLatencyMs);
        b.Property(x => x.LastPollSuccess);

        b.Property(x => x.LastPollErrorCode)
            .HasMaxLength(64);
        
        b.Property(x => x.LastKnownPeersActive)
            .IsRequired();

        b.Property(x => x.LastRxTotal).IsRequired();
        b.Property(x => x.LastTxTotal).IsRequired();
        b.Property(x => x.LastTotalsAtUtc);

        b.Property(x => x.LastRxDelta).IsRequired();
        b.Property(x => x.LastTxDelta).IsRequired();
        b.Property(x => x.LastDeltaAtUtc);
        
        b.Property(x => x.LastUpdaterInstance)
            .HasMaxLength(128);
        
        b.Property(x => x.RowVersion)
            // PostgreSQL не имеет аналога SQL Server rowversion с автогенерацией.
            // В нашей схеме колонка `RowVersion` NOT NULL и без DEFAULT,
            // поэтому EF обязан передавать значение при INSERT.
            .IsConcurrencyToken()
            .IsRequired()
            .ValueGeneratedNever();

        b.Property(x => x.UpdatedAtUtc)
            .IsRequired();
        
        b.HasIndex(x => x.LeaseUntilUtc);
        b.HasIndex(x => x.BackoffUntilUtc);
        
        b.HasIndex(x => x.LastReachableUtc);
    }
}