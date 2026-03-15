using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DrakkarVpn.Servers.Infrastructure.EF;

public sealed class ServersDbContextFactory : IDesignTimeDbContextFactory<ServerDbContext>
{
    public ServerDbContext CreateDbContext(string[] args)
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
            ?? "Host=127.0.0.1;Port=5432;Database=drakkar;Username=postgres;Password=ginger567789";

        var options = new DbContextOptionsBuilder<ServerDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ServerDbContext(options);
    }
}