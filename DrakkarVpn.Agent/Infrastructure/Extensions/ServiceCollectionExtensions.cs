using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Services;
using DrakkarVpn.Agent.Infrastructure.Grpc;
using Grpc.Net.Client;

namespace DrakkarVpn.Agent.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddV2RayGrpc(this IServiceCollection services, IConfiguration cfg)
    {
        var baseAddress = cfg.GetRequiredSection("V2RayTune")["BaseAddress"]
                          ?? throw new InvalidOperationException("V2RayTune:BaseAddress is not configured");

        services.AddSingleton(sp =>
            GrpcChannel.ForAddress(baseAddress, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            }));

        services.AddSingleton<IV2RayGrpcClient, V2RayGrpcClient>();
        services.AddScoped<IV2RayService, V2RayService>();

        return services;
    }
}