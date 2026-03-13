using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Services;
using DrakkarVpn.Agent.Infrastructure.Config;
using DrakkarVpn.Agent.Infrastructure.Services.Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Agent.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
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