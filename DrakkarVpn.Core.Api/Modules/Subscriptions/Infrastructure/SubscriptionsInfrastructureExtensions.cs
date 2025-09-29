using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.Repositories;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure;

public static class SubscriptionsInfrastructureExtensions
{
    public static IServiceCollection AddSubscriptionsInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        return services;
    }
}