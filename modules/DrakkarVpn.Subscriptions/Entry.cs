using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services.Activation;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Subscriptions;

public static class Entry
{
    public static IServiceCollection AddSubscriptionsAdminHost(this IServiceCollection services)
    {
        services.AddScoped<ISubscriptionActivationCore, SubscriptionActivationCore>();
        services.AddScoped<ISubscriptionGrantService, SubscriptionGrantService>();
        services.AddScoped<ISubscriptionCancellationService, SubscriptionCancellationService>();
        services.AddScoped<ISubscriptionExpirationService, SubscriptionExpirationService>();
        
        services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

        return services;
    }
    
    public static IServiceCollection AddSubscriptionsUserHost(this IServiceCollection services)
    {
        services.AddScoped<ISubscriptionActivationCore, SubscriptionActivationCore>();
        services.AddScoped<ISubscriptionPurchaseService, SubscriptionPurchaseService>();
        services.AddScoped<ISubscriptionGrantService, SubscriptionGrantService>();

        services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

        return services;
    }
    
    public static IServiceCollection AddSubscriptionsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");
        services.AddDbContext<SubscriptionDbContext>(o => o.UseNpgsql(cs));
        
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        return services;
    }
    
    
}