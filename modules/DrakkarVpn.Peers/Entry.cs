using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Services;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.BackgroundWorkers;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;
using DrakkarVpn.Peers.Application.Abstractions.Services;
using DrakkarVpn.Peers.Application.Services.Provisioning;
using DrakkarVpn.Peers.Infrastructure.BackgroundWorkers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Peers;

public static class Entry
{
    public static IServiceCollection AddPeersAdminHost(this IServiceCollection services)
    {
        services.AddScoped<IPeersQueryService, PeersQueryService>();
        services.AddScoped<IPeerRevocationService, PeerRevocationService>();
        services.AddScoped<IPeerMetricsProcessor, PeerMetricsProcessor>();
        
        return services;
    }

    public static IServiceCollection AddPeersUserHost(this IServiceCollection services)
    {
        services.AddScoped<IPeersQueryService, PeersQueryService>();
        services.AddScoped<IPeerAgentProvisioningService, PeerAgentProvisioningService>();
        services.AddScoped<IPeerProvisionJobsService, PeerProvisionJobsService>();
        services.AddScoped<IPeerConfigsService, PeerConfigsService>();
        services.AddScoped<IPeerProvisionPayloadFactory, PeerProvisionPayloadFactory>();
        services.AddScoped<IPeersDomainCreateService, PeersDomainCreateService>();
        
        return services;
    }
    
    public static IServiceCollection AddPeersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");
        services.AddDbContext<PeerDbContext>(o => o.UseNpgsql(cs));
        
        services.AddScoped<IPeerRepository, PeerRepository>();
        services.AddScoped<IPeerMetricsHistoryRepository, PeerMetricsHistoryRepository>();
        services.AddScoped<IPeerProvisionJobsRepository, PeerProvisionJobsRepository>();
        services.AddScoped<IPeerTrafficAggRepository, PeerTrafficAggRepository>();
        services.AddScoped<IPeersAgentClient, PeersAgentClient>();
        return services;
    }
    
    public static IServiceCollection AddPeersWorkersAdminHost(this IServiceCollection services)
    {
        services.AddHostedService<PeerMetricsBackgroundWorker>();
        services.AddHostedService<PeerTrafficAggWorker>();
        return services;
    }

    public static IServiceCollection AddPeersWorkersUserHost(this IServiceCollection services)
    {
        services.AddHostedService<PeerProvisioningWorker>();
        return services;
    }
}