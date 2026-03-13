using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace DrakkarVpn.Admin.Api.DI;

public static class HostServiceCollectionExtensions
{
    public static IServiceCollection AddAdminHostApi(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Drakkar Admin API",
                Version = "v1"
            });
        });

        return services;
    }
    
    
    public static IServiceCollection AddLocalCorsForFrontend(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}