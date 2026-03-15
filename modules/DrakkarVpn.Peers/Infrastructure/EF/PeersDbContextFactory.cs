using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DrakkarVpn.Peers.Infrastructure.EF;

public sealed class PeersDbContextFactory : IDesignTimeDbContextFactory<PeerDbContext>
{
    public PeerDbContext CreateDbContext(string[] args)
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

        var options = new DbContextOptionsBuilder<PeerDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new PeerDbContext(options);
    }
}