using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DrakkarVpn.Subscriptions.Infrastructure.EF;

public sealed class SubscriptionsDbContextFactory : IDesignTimeDbContextFactory<SubscriptionDbContext>
{
    public SubscriptionDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("Db")
            ?? "Host=127.0.0.1;Port=5432;Database=drakkar;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<SubscriptionDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new SubscriptionDbContext(options);
    }
}