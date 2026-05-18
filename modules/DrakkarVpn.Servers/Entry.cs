using DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Services;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;
using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.Options;
using DrakkarVpn.Servers.Application.Services;
using DrakkarVpn.Servers.Application.Services.Queries;
using DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply.AgentApply;
using DrakkarVpn.Servers.Infrastructure;
using DrakkarVpn.Servers.Infrastructure.BackgroundWorkers;
using DrakkarVpn.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Infrastructure.EF.Repositories;
using DrakkarVpn.Servers.Infrastructure.EF.Repositories.ServerTransportActivation;
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
        AddServersQueryServices(services);
        services.AddScoped<IServerManagementService, ServerManagementService>();
        services.AddScoped<ITransportProfilesManagementService, TransportProfilesManagementService>();
        services.AddScoped<IServerTransportActivationService, ServerTransportActivationService>();
        services.AddScoped<IServerTransportManagementService, ServerTransportManagementService>();
        services.AddScoped<IServerTransportActivationQueryService, ServerTransportActivationQueryService>();
        services.AddScoped<IAgentPollingService, AgentPollingService>();
        services.AddScoped<IServerPollStateService, ServerPollStateService>();
        services.AddScoped<IServerPollResultApplyService, ServerPollResultApplyService>();
        services.AddScoped<IServerRealtimeStatsUpsertService, ServerRealtimeStatsUpsertService>();
        services.AddScoped<IServerMetricsHistoryService, ServerMetricsHistoryService>();
        services.AddHttpClient<IAgentTransportApiClient, ServerTransportAgentClient>();
        services.AddScoped<IAgentApplyServerTransportRequestBuilder, AgentApplyServerTransportRequestBuilder>();
        services.AddScoped<IAgentApplyServerTransportHttpExecutor, AgentApplyServerTransportHttpExecutor>();
        services.AddScoped<IAgentApplyServerTransportService, AgentApplyServerTransportService>();
        services.AddScoped<IServerTransportApplyJobService, ServerTransportApplyJobService>();
        services.AddScoped<IServerTransportAppliedRecorder, ServerTransportAppliedRecorder>();
        services.AddScoped<IServerTransportApplyJobObsoleteGuard, ServerTransportApplyJobObsoleteGuard>();
        services.AddScoped<IServerTransportApplyJobOutcomeClassifier, ServerTransportApplyJobOutcomeClassifier>();
        services.AddScoped<IServerTransportApplyJobProcessor, ServerTransportApplyJobProcessor>();

        return services;
    }
    
    public static IServiceCollection AddServersUserHost(this IServiceCollection services)
    {
        services.AddScoped<IServerConfigQueryService, ServerConfigQueryService>();
        services.AddScoped<IServerAgentQueryService, ServerAgentQueryService>();
        services.AddScoped<IServerQueryForPeers>(sp => sp.GetRequiredService<IServerAgentQueryService>());
        return services;
    }

    private static void AddServersQueryServices(IServiceCollection services)
    {
        services.AddScoped<IServersQueryService, ServersQueryService>();
        services.AddScoped<IServerTransportStateQueryService, ServerTransportStateQueryService>();
        services.AddScoped<IServerConfigQueryService, ServerConfigQueryService>();
        services.AddScoped<IServerAgentQueryService, ServerAgentQueryService>();
        services.AddScoped<IServerMetricsQueryService, ServerMetricsQueryService>();
        services.AddScoped<IServerQueryForPeers>(sp => sp.GetRequiredService<IServerAgentQueryService>());
    }
    
    public static IServiceCollection AddServersInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");
        services.AddValidatedServerTransportApplyOptions(configuration);
        services.AddDbContext<ServerDbContext>(o => o.UseNpgsql(cs));
        
        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<IServerMetricsHistoryRepository, ServerMetricsHistoryRepository>();
        services.AddScoped<IServerPollStateRepository, ServerPollStateRepository>();
        services.AddScoped<IServerPollResultApplyRepository, ServerPollResultApplyRepository>();
        services.AddScoped<IServerRealtimeStatsRepository, ServerRealtimeStatsRepository>();
        services.AddScoped<ITransportProfileReadRepository, TransportProfileReadRepository>();
        services.AddScoped<ITransportProfileWriteRepository, TransportProfileWriteRepository>();
        services.AddScoped<IServerTransportActivationReadRepository, ServerTransportActivationReadRepository>();
        services.AddScoped<IServerTransportApplyJobRepository, ServerTransportApplyJobRepository>();
        services.AddScoped<IAgentApplyServerTransportContextRepository, AgentApplyServerTransportContextRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
    
    public static IServiceCollection AddServersWorkersAdminHost(this IServiceCollection services)
    {
        services.AddHostedService<ServersPollingBackgroundWorker>();
        services.AddHostedService<ServerMetricsHistoryCleanupWorker>();
        services.AddHostedService<ServerTransportApplyJobWorker>();
        return services;
    }
    
}