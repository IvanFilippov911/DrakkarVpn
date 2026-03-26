using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Services;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;
using DrakkarVpn.Peers.Infrastructure.EF.Repositories;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IPeersAgentClient, PeersAgentClient>();
        services.AddScoped<IPeerTrafficAggRepository, PeerTrafficAggRepository>();
        services.AddScoped<IPeerRevocationService, PeerRevocationService>();
        services.AddScoped<IPeerProvisionJobsRepository, PeerProvisionJobsRepository>();
        return services;
    }
}