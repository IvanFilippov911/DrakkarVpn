using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Services;
using DrakkarVpn.Agent.Infrastructure.Config;
using DrakkarVpn.Agent.Infrastructure.Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Agent.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddV2RayGrpc(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<V2RayOptions>(cfg.GetSection("V2RayOptions"));
        
        services.AddSingleton<IV2RayClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<V2RayOptions>>();
            return new V2RayGrpcClient(opts);
        });
        
        services.AddScoped<IV2RayService, V2RayService>();
        services.AddScoped<IHealthService, HealthService>();

        return services;
    }
}