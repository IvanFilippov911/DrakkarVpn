using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Services;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Extension;

public static class TariffsServiceCollectionExtensions
{
    public static IServiceCollection AddTariffsModule(this IServiceCollection services)
    {
        services.AddScoped<ITariffAdminService, TariffAdminService>();
        
        services.AddScoped<IValidator<TariffCreateDto>, TariffCreateDtoValidator>();
        services.AddScoped<IValidator<TariffUpdateDto>, TariffUpdateDtoValidator>();
        services.AddScoped<ITariffQueryService, TariffQueryService>();

        return services;
    }
}