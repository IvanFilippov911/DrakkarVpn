using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Configurations;

public sealed class CoreErrorEventConfig : IEntityTypeConfiguration<CoreErrorEvent>
{
    public void Configure(EntityTypeBuilder<CoreErrorEvent> b)
    {
        b.ToTable("core_error_events");

        b.HasKey(x => x.Id);

        b.Property(x => x.TimestampUtc).IsRequired();
        b.Property(x => x.Command).HasMaxLength(200).IsRequired();
        b.Property(x => x.Area).HasMaxLength(100).IsRequired();

        b.Property(x => x.ErrorType).HasMaxLength(50).IsRequired();
        b.Property(x => x.DomainCode).HasMaxLength(200);

        b.Property(x => x.Message).HasColumnType("text").IsRequired();
        b.Property(x => x.TraceId).HasMaxLength(100).IsRequired();

        b.Property(x => x.UserId).HasMaxLength(100);
        b.Property(x => x.TelegramId).HasMaxLength(100);

        b.Property(x => x.PayloadJson).HasColumnType("jsonb");
    }
}