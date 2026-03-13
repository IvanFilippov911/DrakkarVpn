using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Services;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Validation;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Tariffs;

public static class Entry
{
    public static IServiceCollection AddTariffsAdminHost(this IServiceCollection services)
    {
        services.AddScoped<ITariffAdminService, TariffAdminService>();
        services.AddScoped<ITariffQueryService, TariffQueryService>();

        services.AddScoped<IValidator<TariffCreateDto>, TariffCreateDtoValidator>();
        services.AddScoped<IValidator<TariffUpdateDto>, TariffUpdateDtoValidator>();

        return services;
    }
    
    public static IServiceCollection AddTariffsUserHost(this IServiceCollection services)
    {
        services.AddScoped<ITariffQueryService, TariffQueryService>();

        return services;
    }
    
    public static IServiceCollection AddTariffsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");
        services.AddDbContext<TariffsDbContext>(o => o.UseNpgsql(cs));
        services.AddScoped<ITariffRepository, TariffRepository>();
        return services;
    }
    
    
}