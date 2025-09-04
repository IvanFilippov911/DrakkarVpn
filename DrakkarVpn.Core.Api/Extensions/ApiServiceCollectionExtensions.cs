using Microsoft.OpenApi.Models;

namespace DrakkarVpn.Core.Api.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiBasics(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Drakkar API", Version = "v1" });
        });
        return services;
    }
}