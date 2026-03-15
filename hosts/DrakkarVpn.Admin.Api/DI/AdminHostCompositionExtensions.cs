using DrakkarVpn.AdminAuth;
using DrakkarVpn.Observability;
using DrakkarVpn.Peers;
using DrakkarVpn.Servers;
using DrakkarVpn.Subscriptions;
using DrakkarVpn.Tariffs;
using DrakkarVpn.Users;

namespace DrakkarVpn.Admin.Api.DI;

public static class AdminHostCompositionExtensions
{
    public static IServiceCollection AddAdminHostComposition(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // shared infra
        services.AddHttpClient();
        services.AddRedis(configuration);

        // execution
        //services.AddExecutionModule();
        //services.AddExecutionPipelines();

        // modules infrastructure
        services.AddAdminAuthInfrastructure(configuration);
        services.AddObservabilityInfrastructure(configuration);
        services.AddUsersInfrastructure(configuration);
        services.AddPeersInfrastructure(configuration);
        services.AddServersInfrastructure(configuration);
        services.AddSubscriptionsInfrastructure(configuration);
        services.AddTariffsInfrastructure(configuration);

        // host-specific module services
        services.AddAdminAuthAdminHost(configuration);
        services.AddObservabilityAdminHost();
        services.AddUsersAdminHost();
        services.AddPeersAdminHost();
        services.AddServersAdminHost();
        services.AddSubscriptionsAdminHost();
        services.AddTariffsAdminHost();

        // pipelines
        services.AddAdminHostPipelines();

        // workers
        services.AddObservabilityWorkersAdminHost();
        //services.AddPeersWorkersAdminHost();
        services.AddServersWorkersAdminHost();

        return services;
    }
}