using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.AdminAuth.Application.Abstractions;
using DrakkarVpn.AdminAuth.Infrastructure.EF;
using DrakkarVpn.Execution.Application.Pipelines;
using DrakkarVpn.Peers.Infrastructure.EF;
using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Infrastructure.EF;
using DrakkarVpn.Subscriptions.Infrastructure.EF;
using DrakkarVpn.Tariffs.Infrastructure.EF;
using DrakkarVpn.Users.Infrastructure.EF;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetworkMonitoring.Application.Abstractions;

namespace DrakkarVpn.HostInfrastructure;

public static class Entry
{
    public static IServiceCollection AddUserHostUnitOfWorkBehaviors(this IServiceCollection services)
    {
        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
        services.AddScoped<ISubscriptionsUnitOfWork, SubscriptionsUnitOfWork>();
        services.AddScoped<IPeersUnitOfWork, PeersUnitOfWork>();
        services.AddScoped<IServersUnitOfWork, ServersUnitOfWork>();
        services.AddScoped<ITariffsUnitOfWork, TariffsUnitOfWork>();
        services.AddScoped<INetworkMonitoringUnitOfWork, NetworkMonitoring.Infrastructure.EF.NetworkMonitoringUnitOfWork>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UsersUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SubscriptionsUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PeersUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ServersUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TariffsUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NetworkMonitoringUnitOfWorkBehavior<,>));

        return services;
    }

    public static IServiceCollection AddAdminHostUnitOfWorkBehaviors(this IServiceCollection services)
    {
        services.AddScoped<IAdminAuthUnitOfWork, AdminAuthUnitOfWork>();
        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
        services.AddScoped<ISubscriptionsUnitOfWork, SubscriptionsUnitOfWork>();
        services.AddScoped<IPeersUnitOfWork, PeersUnitOfWork>();
        services.AddScoped<IServersUnitOfWork, ServersUnitOfWork>();
        services.AddScoped<ITariffsUnitOfWork, TariffsUnitOfWork>();
        services.AddScoped<INetworkMonitoringUnitOfWork, NetworkMonitoring.Infrastructure.EF.NetworkMonitoringUnitOfWork>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AdminAuthUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UsersUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SubscriptionsUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PeersUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ServersUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TariffsUnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NetworkMonitoringUnitOfWorkBehavior<,>));

        return services;
    }
}