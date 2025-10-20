using System.Reflection;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application;
using DrakkarVpn.Core.Api.Modules.Peers.Application;
using DrakkarVpn.Core.Api.Modules.Servers.Application;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application;
using DrakkarVpn.Core.Api.Modules.Users.Application;
using DrakkarVpn.Core.Pipeline;
using MediatR;

namespace DrakkarVpn.Core.Api.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddDrakkarMediatR(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(OrchestratorApplicationMarker).Assembly,
                typeof(SubscriptionsApplicationMarker).Assembly,
                typeof(ServerApplicationMarker).Assembly,
                typeof(UsersApplicationMarker).Assembly,
                typeof(PeersApplicationMarker).Assembly
            );

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            //cfg.AddOpenBehavior(typeof(IdempotencyBehavior<,>));
            cfg.AddOpenBehavior(typeof(EfTransactionBehavior<,>));
            
        });
        

        return services;
    }
}