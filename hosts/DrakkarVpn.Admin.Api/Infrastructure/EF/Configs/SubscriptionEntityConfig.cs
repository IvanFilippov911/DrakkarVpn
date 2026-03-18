using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Configurations;

public sealed class SubscriptionRowConfig : IEntityTypeConfiguration<SubscriptionReadEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionReadEntity> b)
    {
        b.ToTable("subscriptions", schema: "subscriptions");

        b.HasKey(x => x.Id);

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.UserId, x.EndAtUtc });

        b.Property(x => x.StartAtUtc).HasColumnName("start_at");
        b.Property(x => x.EndAtUtc).HasColumnName("end_at");
        b.Property(x => x.StatusUpdatedAtUtc).HasColumnName("status_updated_at_utc");

        b.Property(x => x.Status)
            .HasConversion<int>()
            .HasColumnName("status");

        b.Property(x => x.MaxDevices).HasColumnName("max_devices");
    }
}