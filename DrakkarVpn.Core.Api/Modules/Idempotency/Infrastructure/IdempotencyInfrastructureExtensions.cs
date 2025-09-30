using DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Idempotency.Infrastructure.Repositories;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

namespace DrakkarVpn.Core.Api.Modules.Idempotency.Infrastructure;

public static class IdempotencyInfrastructureExtensions
{
    public static IServiceCollection AddIdempotencyInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        return services;
    }
}