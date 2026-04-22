using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Entities;
using DrakkarVpn.Servers.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class ServerTransportActivationConfiguration : IEntityTypeConfiguration<ServerTransportActivation>
{
    public void Configure(EntityTypeBuilder<ServerTransportActivation> b)
    {
        b.ToTable("server_transport_activations", "servers");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.ServerId)
            .HasColumnName("server_id")
            .IsRequired();

        b.Property(x => x.TransportProfileId)
            .HasColumnName("transport_profile_id")
            .IsRequired();

        b.Property(x => x.RealityPublicKey)
            .HasColumnName("reality_public_key")
            .HasMaxLength(ServerTransportActivation.RealityPublicKeyMaxLength)
            .IsRequired();

        b.Property(x => x.LocalPriority)
            .HasColumnName("local_priority")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz")
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz")
            .IsRequired();

        b.Property(x => x.ActivatedAtUtc)
            .HasColumnName("activated_at_utc")
            .HasColumnType("timestamptz");

        b.HasIndex(x => new { x.ServerId, x.LocalPriority })
            .HasDatabaseName("ix_server_transport_activations_server_id_local_priority");

        b.HasIndex(x => new { x.ServerId, x.TransportProfileId })
            .IsUnique()
            .HasDatabaseName("ux_server_transport_activations_server_profile");

        b.HasIndex(x => x.ServerId)
            .IsUnique()
            .HasFilter($"status = {(int)TransportActivationStatus.Active}")
            .HasDatabaseName("ux_server_transport_activations_one_active_per_server");

        b.HasOne<TransportProfile>()
            .WithMany()
            .HasForeignKey(x => x.TransportProfileId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}