using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DrakkarVpn.Tariffs.Infrastructure.EF;

public sealed class TariffsDbContextFactory : IDesignTimeDbContextFactory<TariffsDbContext>
{
    public TariffsDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Db")
            ?? "Host=127.0.0.1;Port=5432;Database=drakkar;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<TariffsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TariffsDbContext(options);
    }
}