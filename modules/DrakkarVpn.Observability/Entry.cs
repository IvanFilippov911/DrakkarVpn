
using DrakkarVpn.Core.Api.Diagnostics;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Services.Alerts.CoreHealth;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Repository;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using DrakkarVpn.Observability.Application.Features.Services.Alerts;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.CoreHealth;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Exceptions;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.PeerTraffic;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Servers;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.ServersPolling;
using DrakkarVpn.Observability.Application.Features.Services.ErrorEvents;
using DrakkarVpn.Observability.Application.Services.Alerts;
using DrakkarVpn.Observability.Application.Services.Alerts.Factories.Exceptions;
using DrakkarVpn.Observability.Application.Services.Diagnostics;
using DrakkarVpn.Observability.Infrastructure.EF;
using DrakkarVpn.Observability.Infrastructure.EF.Repositories;
using DrakkarVpn.Observability.Infrastructure.Pipelines;
using DrakkarVpn.Observability.Infrastructure.Telemetry;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DrakkarVpn.Observability;

public static class Entry
{
    public static IServiceCollection AddObservabilityAdminHost(this IServiceCollection services)
    {
        services.AddScoped<ICoreAlertService, CoreAlertService>();
        services.AddScoped<ICoreAlertsQueryService, CoreAlertsQueryService>();

        services.AddScoped<ICoreErrorEventService, CoreErrorEventService>();
        services.AddScoped<ICoreErrorEventsQueryService, CoreErrorEventsQueryService>();

        services.AddScoped<ICoreHealthQueryService, CoreHealthQueryService>();
        
        services.AddSingleton<CoreRequestMetricsBuffer>();
        
        services.AddSingleton<ICoreRequestMetricsSink>(sp =>
            sp.GetRequiredService<CoreRequestMetricsBuffer>());
        
        services.AddScoped<IExceptionAlertFactory, ExceptionAlertFactory>();
        services.AddScoped<ICoreHealthAlertFactory, CoreHealthAlertFactory>();
        services.AddScoped<IPeerTrafficAlertFactory, PeerTrafficAlertFactory>();
        services.AddScoped<IServerAlertFactory, ServerAlertFactory>();
        services.AddScoped<IServersPollingAlertFactory, ServersPollingAlertFactory>();
        
        services.AddSingleton<IRequestTelemetryContextAccessor, RequestTelemetryContextAccessor>();
        
        return services;
    }

    public static IServiceCollection AddObservabilityUserHost(this IServiceCollection services)
    {
        services.AddSingleton<IRequestTelemetryContextAccessor, RequestTelemetryContextAccessor>();
        services.AddScoped<ICoreAlertService, CoreAlertService>();
        services.AddScoped<ICoreErrorEventService, CoreErrorEventService>();

        services.AddScoped<IExceptionAlertFactory, ExceptionAlertFactory>();

        services.AddSingleton<CoreRequestMetricsBuffer>();
        services.AddSingleton<ICoreRequestMetricsSink>(sp =>
            sp.GetRequiredService<CoreRequestMetricsBuffer>());
        return services;
    }
    
    public static IServiceCollection AddObservabilityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");

        services.AddDbContext<ObservabilityDbContext>(o => o.UseNpgsql(cs));

        services.AddScoped<ICoreAlertRepository, CoreAlertRepository>();
        services.AddScoped<ICoreErrorEventRepository, CoreErrorEventRepository>();
        return services;
    }
    
    public static IServiceCollection AddObservabilityWorkersAdminHost(this IServiceCollection services)
    {
        services.AddHostedService<CoreMetricsBackgroundService>();
        return services;
    }
    
    public static IServiceCollection AddUserHostPipelines(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetryContextBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorEventBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AlertingBehavior<,>));

        return services;
    }

    public static IServiceCollection AddAdminHostPipelines(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetryContextBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorEventBehavior<,>));

        return services;
    }
    
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Redis") ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(cs);
            options.AbortOnConnectFail = false;
            options.ConnectRetry = 3;
            options.ConnectTimeout = 5000;

            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}