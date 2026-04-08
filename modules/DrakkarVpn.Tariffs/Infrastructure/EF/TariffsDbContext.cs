using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;

public sealed class TariffsDbContext : DbContext
{
    public TariffsDbContext(DbContextOptions<TariffsDbContext> options) : base(options) {}

    public DbSet<Tariff> Tariffs => Set<Tariff>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("tariffs");
        b.Entity<Tariff>(e =>
        {
            e.ToTable("tariffs");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .HasConversion(
                    v => v.Value,            
                    v => new TariffId(v));   

            e.Property(x => x.Name).HasMaxLength(128).IsRequired();
            e.Property(x => x.Price).HasColumnType("numeric(18,2)");
            e.Property(x => x.DefaultMaxDevices);
            
            e.Property(x => x.Duration)
                .HasConversion(
                    v => v.Ticks,
                    v => TimeSpan.FromTicks(v));

            e.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            e.Property(x => x.Kind)
                .HasConversion<int>();

            e.Property(x => x.CreatedAt);
            e.Property(x => x.UpdatedAt);
        });
    }
}