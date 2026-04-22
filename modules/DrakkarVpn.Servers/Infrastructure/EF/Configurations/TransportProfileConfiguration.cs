using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Servers.Infrastructure.EF.Configurations;

public sealed class TransportProfileConfiguration : IEntityTypeConfiguration<TransportProfile>
{
    public void Configure(EntityTypeBuilder<TransportProfile> b)
    {
        b.ToTable("transport_profiles", "servers");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();
        
        b.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(TransportProfile.NameMaxLength);

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
            .HasMaxLength(TransportProfile.RealitySniMaxLength);

        b.Property(x => x.RealityShortId)
            .HasColumnName("reality_short_id")
            .HasMaxLength(TransportProfile.RealityShortIdMaxLength);

        b.Property(x => x.RealityFingerprint)
            .HasColumnName("reality_fingerprint")
            .HasMaxLength(TransportProfile.RealityFingerprintMaxLength);
        
        b.Property(x => x.RealityDest)
            .HasColumnName("reality_dest")
            .HasMaxLength(TransportProfile.RealityDestMaxLength);

        b.Property(x => x.GrpcServiceName)
            .HasColumnName("grpc_service_name")
            .HasMaxLength(TransportProfile.GrpcFieldMaxLength);

        b.Property(x => x.GrpcAuthority)
            .HasColumnName("grpc_authority")
            .HasMaxLength(TransportProfile.GrpcFieldMaxLength);

        b.Property(x => x.GlobalPriority)
            .HasColumnName("global_priority")
            .IsRequired();

        b.Property(x => x.IsEnabled)
            .HasColumnName("is_enabled")
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
        
        b.HasIndex(x => x.Name)
            .HasDatabaseName("ix_transport_profiles_name");

        b.HasIndex(x => new { x.IsEnabled, x.GlobalPriority })
            .HasDatabaseName("ix_transport_profiles_enabled_priority");
        
    }
}
