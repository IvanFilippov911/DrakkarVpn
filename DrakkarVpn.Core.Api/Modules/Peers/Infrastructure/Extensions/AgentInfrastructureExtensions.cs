using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure;

public static class AgentInfrastructureExtensions
{
    public static IServiceCollection AddAgentInfrastructure(this IServiceCollection services)
    {
        
        services.AddHttpClient<IPeersAgentClient, PeersAgentClient>();

        return services;
    }
}