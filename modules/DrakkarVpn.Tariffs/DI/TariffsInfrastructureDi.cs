using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Tariffs.DI;

public static class TariffsInfrastructureDi
{
    public static IServiceCollection AddTariffsInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<ITariffRepository, TariffRepository>();

        return services;
    }
}