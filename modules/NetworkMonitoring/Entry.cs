using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetworkMonitoring.Application.Abstractions.Services;
using NetworkMonitoring.Application.Services;
using NetworkMonitoring.Infrastructure.EF;
using NetworkMonitoring.Infrastructure.EF.Repositories;
using NetworkMonitoring.Application.Abstractions.Repositories;

namespace NetworkMonitoring;

public static class Entry
{
    public static IServiceCollection AddNetworkMonitoringAdminHost(this IServiceCollection services)
    {
        services.AddScoped<IProbeNodeRegistrationService, ProbeNodeRegistrationService>();
        services.AddScoped<IProbeNodesQueryService, ProbeNodesQueryService>();
        services.AddScoped<IProbeNodeManagementService, ProbeNodeManagementService>();
        return services;
    }

    public static IServiceCollection AddNetworkMonitoringInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");

        services.AddDbContext<NetworkMonitoringDbContext>(o => o.UseNpgsql(cs));

        services.AddScoped<IProbeNodeWriteRepository, ProbeNodeWriteRepository>();
        services.AddScoped<IProbeNodeReadRepository, ProbeNodeReadRepository>();

        return services;
    }
}

