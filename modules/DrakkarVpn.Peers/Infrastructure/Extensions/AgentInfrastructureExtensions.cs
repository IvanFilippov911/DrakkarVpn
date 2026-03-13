using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure;

public static class AgentInfrastructureExtensions
{
    public static IServiceCollection AddAgentInfrastructure(this IServiceCollection services)
    {
        
        services.AddHttpClient<IPeersAgentClient, PeersAgentClient>();

        return services;
    }
}