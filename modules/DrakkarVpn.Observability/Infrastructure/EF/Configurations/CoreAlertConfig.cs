using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Configurations;

public sealed class CoreAlertConfig : IEntityTypeConfiguration<CoreAlert>
{
    public void Configure(EntityTypeBuilder<CoreAlert> b)
    {
        b.ToTable("core_alerts");

        b.HasKey(x => x.Id);

        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.ResolvedAtUtc);
        b.Property(x => x.IsResolved).IsRequired();

        b.Property(x => x.Source)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Code)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Severity)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        b.Property(x => x.Message)
            .HasColumnType("text")
            .IsRequired();

        b.Property(x => x.ServerId);
        b.Property(x => x.UserId);
        b.Property(x => x.Region)
            .HasMaxLength(50);

        b.Property(x => x.DetailsJson)
            .HasColumnType("jsonb");
        

        b.Property(x => x.ResolutionType)
            .HasMaxLength(50)
            .HasColumnName("resolution_type")
            .HasConversion<string>();   

        b.Property(x => x.ResolutionNote)
            .HasColumnName("resolution_note")
            .HasColumnType("text");

        b.Property(x => x.ResolvedByAdminId)
            .HasColumnName("resolved_by_admin_id");

        
        b.HasIndex(x => x.CreatedAtUtc);
        b.HasIndex(x => x.IsResolved);
        b.HasIndex(x => new { x.IsResolved, x.CreatedAtUtc });
        b.HasIndex(x => x.ServerId);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ResolvedByAdminId);
    }
}