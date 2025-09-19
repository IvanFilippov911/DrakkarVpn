using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure;

public static class PeersInfrastructureExtensions
{
    public static IServiceCollection AddPeersInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<IPeerRepository, PeerRepository>();

        return services;
    }
}