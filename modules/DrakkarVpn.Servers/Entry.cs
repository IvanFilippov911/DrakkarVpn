using DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Services;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Services;
using DrakkarVpn.Servers.Infrastructure.Time;
using DrakkarVpn.Shared.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Servers;

public static class Entry
{
    public static IServiceCollection AddServersAdminHost(this IServiceCollection services)
    {
        services.AddScoped<IServersQueryService, ServersQueryService>();
        services.AddScoped<IServerManagementService, ServerManagementService>();
        services.AddScoped<ITransportProfilesManagementService, TransportProfilesManagementService>();
        services.AddScoped<IServerTransportActivationManagementService, ServerTransportActivationManagementService>();
        services.AddScoped<IAgentPollingService, AgentPollingService>();
        services.AddScoped<IServerPollStateService, ServerPollStateService>();
        services.AddScoped<IServerPollResultApplyService, ServerPollResultApplyService>();
        services.AddScoped<IServerRealtimeStatsUpsertService, ServerRealtimeStatsUpsertService>();
        services.AddScoped<IServerMetricsHistoryService, ServerMetricsHistoryService>();
        services.AddScoped<IServerQueryForPeers, ServersQueryService>();
        
        return services;
    }
    
    public static IServiceCollection AddServersUserHost(this IServiceCollection services)
    {
        services.AddScoped<IServersQueryService, ServersQueryService>();
        services.AddScoped<IServerQueryForPeers, ServersQueryService>();
        return services;
    }
    
    public static IServiceCollection AddServersInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");
        services.AddDbContext<ServerDbContext>(o => o.UseNpgsql(cs));
        
        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<IServerMetricsHistoryRepository, ServerMetricsHistoryRepository>();
        services.AddScoped<IServerPollStateRepository, ServerPollStateRepository>();
        services.AddScoped<IServerPollResultApplyRepository, ServerPollResultApplyRepository>();
        services.AddScoped<IServerRealtimeStatsRepository, ServerRealtimeStatsRepository>();
        services.AddScoped<ITransportProfileReadRepository, TransportProfileReadRepository>();
        services.AddScoped<ITransportProfileWriteRepository, TransportProfileWriteRepository>();
        services.AddScoped<IServerTransportActivationReadRepository, ServerTransportActivationReadRepository>();
        services.AddScoped<IServerTransportActivationWriteRepository, ServerTransportActivationWriteRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IServersAgentClient, ServersAgentClient>();

        return services;
    }
    
    public static IServiceCollection AddServersWorkersAdminHost(this IServiceCollection services)
    {
        services.AddHostedService<ServersPollingBackgroundWorker>();
        services.AddHostedService<ServerMetricsHistoryCleanupWorker>();
        return services;
    }
    
}