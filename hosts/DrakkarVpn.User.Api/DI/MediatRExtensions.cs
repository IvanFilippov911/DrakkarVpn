
using DrakkarVpn.Core.Api.Modules.Peers.Application.Handlers.PeerCreate;
using MediatR;

namespace DrakkarVpn.Core.Api.DI;

public static class MediatRExtensions
{
    public static IServiceCollection AddDrakkarMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            services.AddTransient<IRequestHandler<PeerCreateCommand, Unit>, PeerCreateHandler>();
        });

        return services;
    }
}