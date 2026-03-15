using DrakkarVpn.AdminAuth.Infrastructure.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.AdminAuth.DI;

public static class AdminAuthBootstrapExtensions
{
    public static async Task SeedAdminAuthAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AdminAuthBootstrapSeeder>();
        await seeder.SeedAsync(ct);
    }
}
