using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Infrastructure.EF.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;

public static class ServersInfrastructureExtensions
{
    public static IServiceCollection AddServersInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<IServerMetricsHistoryRepository, ServerMetricsHistoryRepository>();


        return services;
    }
}