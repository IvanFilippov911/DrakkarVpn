using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Options;
using DrakkarVpn.AdminAuth.Infrastructure.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.AdminAuth.DI;

public static class AdminAuthJwtServiceCollectionExtensions
{
    public static IServiceCollection AddAdminAuthJwtInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(AdminJwtOptions.SectionName);

        services.AddOptions<AdminJwtOptions>()
            .Configure(options =>
            {
                options.SigningKey = section["SigningKey"] ?? string.Empty;
                options.Issuer = section["Issuer"] ?? string.Empty;
                options.Audience = section["Audience"] ?? string.Empty;

                if (int.TryParse(section["AccessTokenTtlMinutes"], out var ttlMinutes) && ttlMinutes > 0)
                    options.AccessTokenTtlMinutes = ttlMinutes;
            })
            .Validate(options => !string.IsNullOrWhiteSpace(options.SigningKey), "AdminJwt:SigningKey is missing")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "AdminJwt:Issuer is missing")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "AdminJwt:Audience is missing")
            .Validate(options => options.AccessTokenTtlMinutes > 0, "AdminJwt:AccessTokenTtlMinutes must be > 0")
            .ValidateOnStart();

        services.AddSingleton<IAdminJwtTokenService, AdminJwtTokenService>();

        return services;
    }
}
