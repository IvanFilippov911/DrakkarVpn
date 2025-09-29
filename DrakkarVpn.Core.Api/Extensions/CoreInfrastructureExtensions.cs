using DrakkarVpn.Core.Api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Extensions;

public static class CoreInfrastructureExtensions
{
    public static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'Db' is not configured.");

        services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(connectionString),
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);


        return services;
    }
}
