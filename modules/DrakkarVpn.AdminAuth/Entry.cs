using DrakkarVpn.AdminAuth.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.AdminAuth;

public static class Entry
{
    public static IServiceCollection AddAdminAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddAdminAuthInfrastructureServices(configuration);
    }

    public static IServiceCollection AddAdminAuthAdminHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddAdminAuthAdminHostServices(configuration);
    }
}
