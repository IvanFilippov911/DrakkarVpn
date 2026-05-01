using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.Services;
using DrakkarVpn.Agent.Infrastructure.Config;
using DrakkarVpn.Agent.Infrastructure.EF;
using DrakkarVpn.Agent.Infrastructure.EF.Repositories;
using DrakkarVpn.Agent.Infrastructure.Services.Grpc;
using DrakkarVpn.Agent.Infrastructure.Services.Transport;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Agent.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAgentStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Agent") ?? "Data Source=/var/lib/drakkar-agent/agent.db";

        services.AddDbContext<AgentDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IAgentTransportStateRepository, AgentTransportStateRepository>();
        services.AddScoped<IAgentTransportStateService, AgentTransportStateService>();
        services.AddSingleton<IAgentTransportPayloadHashService, AgentTransportPayloadHashService>();
        services.AddScoped<IXrayTransportConfigApplyService, XrayTransportConfigApplyService>();
        services.AddScoped<IAgentTransportApplyService, AgentTransportApplyService>();

        return services;
    }

    public static IServiceCollection AddXrayGrpc(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<XrayOptions>(cfg.GetSection("XRayOptions"));
        services.AddSingleton<IGrpcChannelProvider, GrpcChannelProvider>();
        services.AddSingleton<IXrayPeerClient>(sp =>
        {
            var channels = sp.GetRequiredService<IGrpcChannelProvider>();
            var opts     = sp.GetRequiredService<IOptions<XrayOptions>>();
            return new XrayPeerGrpcClient(channels, opts);
        });
        
        services.AddSingleton<IXrayStatsClient>(sp =>
        {
            var channels = sp.GetRequiredService<IGrpcChannelProvider>();
            var opts     = sp.GetRequiredService<IOptions<XrayOptions>>();
            return new XrayStatsGrpcClient(channels, opts);
        });

        return services;
    }
}