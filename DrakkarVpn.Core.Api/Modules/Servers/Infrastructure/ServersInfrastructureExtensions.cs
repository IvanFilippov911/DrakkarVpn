using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;

public static class ServersInfrastructureExtensions
{
    public static IServiceCollection AddServersInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddHttpClient<IServersAgentClient, ServersAgentClient>();
        services.AddScoped<IServerMetricsHistoryRepository, ServerMetricsHistoryRepository>();


        return services;
    }
}