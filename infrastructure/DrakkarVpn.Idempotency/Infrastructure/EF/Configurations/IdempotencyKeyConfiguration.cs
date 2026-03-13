using DrakkarVpn.Core.Api.Modules.Idempotency.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Idempotency.Infrastructure.EF.Configurations;

internal sealed class IdempotencyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.ToTable("IdempotencyKeys");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActorKey).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(100);
        builder.Property(x => x.RequestId).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion<int>();
        builder.Property(x => x.CreatedAt).IsRequired();
        
        builder.HasIndex(x => new { x.ActorKey, x.Action, x.RequestId })
            .IsUnique();
    }
}