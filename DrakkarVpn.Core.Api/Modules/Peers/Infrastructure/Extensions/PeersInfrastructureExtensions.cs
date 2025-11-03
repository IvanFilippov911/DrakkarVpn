using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Services;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure;

public static class PeersInfrastructureExtensions
{
    public static IServiceCollection AddPeersInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<IPeerRepository, PeerRepository>();
        services.AddScoped<IPeerMetricsHistoryRepository, PeerMetricsHistoryRepository>();
        services.AddScoped<IPeerMetricsProcessor, PeerMetricsProcessor>();
        services.AddScoped<IPeerSyncIssueRepository, PeerSyncIssueRepository>();


        return services;
    }
}