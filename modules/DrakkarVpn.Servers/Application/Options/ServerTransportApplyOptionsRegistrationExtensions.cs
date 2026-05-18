using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Application.Options;

public static class ServerTransportApplyOptionsRegistrationExtensions
{
    public static IServiceCollection AddValidatedServerTransportApplyOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<ServerTransportApplyJobOptions>, ServerTransportApplyJobOptionsValidator>());
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<ServerTransportAgentApplyOptions>, ServerTransportAgentApplyOptionsValidator>());

        services.AddOptions<ServerTransportApplyJobOptions>()
            .Bind(configuration.GetSection(ServerTransportApplyJobOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<ServerTransportAgentApplyOptions>()
            .Bind(configuration.GetSection(ServerTransportAgentApplyOptions.SectionName))
            .ValidateOnStart();

        return services;
    }
}
