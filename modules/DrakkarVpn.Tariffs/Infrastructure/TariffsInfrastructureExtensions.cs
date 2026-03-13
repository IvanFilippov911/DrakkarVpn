using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure;

public static class TariffsInfrastructureExtensions
{
    public static IServiceCollection AddTariffsInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<ITariffRepository, TariffRepository>();

        return services;
    }
}