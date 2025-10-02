using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, v => new SubscriptionId(v))
            .ValueGeneratedNever();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.StartAt).IsRequired();
        builder.Property(x => x.EndAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("\"Status\" = 1");
    }
}