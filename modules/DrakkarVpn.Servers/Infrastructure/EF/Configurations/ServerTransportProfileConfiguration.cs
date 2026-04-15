using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class ServerTransportProfileConfiguration : IEntityTypeConfiguration<ServerTransportProfile>
{
    public void Configure(EntityTypeBuilder<ServerTransportProfile> b)
    {
        b.ToTable("server_transport_profiles");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.ServerId)
            .HasColumnName("server_id")
            .IsRequired();

        b.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(ServerTransportProfile.NameMaxLength);

        b.Property(x => x.TransportType)
            .HasColumnName("transport_type")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.SecurityType)
            .HasColumnName("security_type")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.RealitySni)
            .HasColumnName("reality_sni")
            .HasMaxLength(ServerTransportProfile.RealitySniMaxLength);

        b.Property(x => x.RealityShortId)
            .HasColumnName("reality_short_id")
            .HasMaxLength(ServerTransportProfile.RealityShortIdMaxLength);

        b.Property(x => x.RealityFingerprint)
            .HasColumnName("reality_fingerprint")
            .HasMaxLength(ServerTransportProfile.RealityFingerprintMaxLength);

        b.Property(x => x.RealityPublicKey)
            .HasColumnName("reality_public_key")
            .HasMaxLength(ServerTransportProfile.RealityPublicKeyMaxLength);

        b.Property(x => x.RealityDest)
            .HasColumnName("reality_dest")
            .HasMaxLength(ServerTransportProfile.RealityDestMaxLength);

        b.Property(x => x.GrpcServiceName)
            .HasColumnName("grpc_service_name")
            .HasMaxLength(ServerTransportProfile.GrpcFieldMaxLength);

        b.Property(x => x.GrpcAuthority)
            .HasColumnName("grpc_authority")
            .HasMaxLength(ServerTransportProfile.GrpcFieldMaxLength);

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Priority)
            .HasColumnName("priority")
            .IsRequired();

        b.Property(x => x.Version)
            .HasColumnName("version")
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

        b.HasIndex(x => new { x.ServerId, x.Priority })
            .HasDatabaseName("ix_server_transport_profiles_server_id_priority");

        b.HasIndex(x => x.ServerId)
            .IsUnique()
            .HasFilter($"status = {(int)TransportProfileStatus.Active}")
            .HasDatabaseName("ux_server_transport_profiles_one_active_per_server");

        b.HasOne<Server>()
            .WithMany("_transportProfiles")
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
