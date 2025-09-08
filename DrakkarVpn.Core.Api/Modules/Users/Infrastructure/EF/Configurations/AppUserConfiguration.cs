using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.EF.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.ToTable("app_users");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.TelegramId)
            .HasColumnName("telegram_id")
            .HasConversion(id => id.Value, v => new TelegramId(v))
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.HasIndex(x => x.TelegramId).IsUnique();
    }
}