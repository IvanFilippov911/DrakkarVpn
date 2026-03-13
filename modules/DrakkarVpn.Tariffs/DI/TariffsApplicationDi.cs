using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Services;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Tariffs.DI;

public static class TariffsApplicationDi
{
    public static IServiceCollection AddTariffsApplication(this IServiceCollection services)
    {
        services.AddScoped<ITariffAdminService, TariffAdminService>();
        services.AddScoped<ITariffQueryService, TariffQueryService>();

        services.AddScoped<IValidator<TariffCreateDto>, TariffCreateDtoValidator>();
        services.AddScoped<IValidator<TariffUpdateDto>, TariffUpdateDtoValidator>();

        return services;
    }
}