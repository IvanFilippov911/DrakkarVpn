using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Authentication;
using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.AdminAuth.Application.Exceptions;
using DrakkarVpn.AdminAuth.Application.Options;
using DrakkarVpn.AdminAuth.Infrastructure.Auth;
using DrakkarVpn.AdminAuth.Infrastructure.Bootstrap;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DrakkarVpn.AdminAuth.DI;

public static class AdminAuthAdminHostServiceCollectionExtensions
{
    public static IServiceCollection AddAdminAuthAdminHostServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddAdminAuthJwtInfrastructure(configuration);
        services.AddAdminAuthBearerAuthentication(configuration);

        var section = configuration.GetSection(AdminRefreshTokenOptions.SectionName);

        services.AddOptions<AdminRefreshTokenOptions>()
            .Configure(options =>
            {
                if (int.TryParse(section["LifetimeDays"], out var lifetimeDays) && lifetimeDays > 0)
                    options.LifetimeDays = lifetimeDays;
            })
            .Validate(options => options.LifetimeDays > 0, "AdminRefreshToken:LifetimeDays must be > 0")
            .ValidateOnStart();

        services.AddOptions<AdminBootstrapOptions>()
            .Configure(options =>
            {
                var bootstrapSection = configuration.GetSection(AdminBootstrapOptions.SectionName);
                options.Email = configuration["ADMIN_BOOTSTRAP_EMAIL"] ?? bootstrapSection["Email"];
                options.Password = configuration["ADMIN_BOOTSTRAP_PASSWORD"] ?? bootstrapSection["Password"];
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentAdminAccessor, CurrentAdminAccessor>();
        services.AddScoped<IAdminAuthenticationService, AdminAuthenticationService>();
        services.AddScoped<IAdminRefreshSessionService, AdminRefreshSessionService>();
        services.AddScoped<IAdminCurrentProfileService, AdminCurrentProfileService>();
        services.AddSingleton<AdminClaimsPrincipalFactory>();
        services.AddSingleton<IAdminProfileFactory, AdminProfileFactory>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<AdminAuthBootstrapSeeder>();

        return services;
    }

    private static IServiceCollection AddAdminAuthBearerAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(AdminJwtOptions.SectionName);
        var options = section.Get<AdminJwtOptions>() ?? new AdminJwtOptions();
        var keyBytes = TryFromBase64(options.SigningKey) ?? Encoding.UTF8.GetBytes(options.SigningKey);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException("AdminJwt:SigningKey must be at least 32 bytes (256-bit). Use Base64 string.");

        var securityKey = new SymmetricSecurityKey(keyBytes);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var principal = context.Principal;
                        var subject = principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (!Guid.TryParse(subject, out var adminId))
                        {
                            context.Fail("Invalid admin token");
                            return;
                        }

                        var tokenId = principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        var requestServices = context.HttpContext.RequestServices;
                        var authenticationService = requestServices.GetRequiredService<IAdminAuthenticationService>();
                        var claimsPrincipalFactory = requestServices.GetRequiredService<AdminClaimsPrincipalFactory>();

                        try
                        {
                            var admin = await authenticationService.GetActiveAdminAsync(
                                adminId,
                                context.HttpContext.RequestAborted);

                            context.Principal = claimsPrincipalFactory.Create(
                                admin,
                                context.Scheme.Name,
                                tokenId);
                        }
                        catch (InvalidAdminCredentialsException ex)
                        {
                            context.Fail(ex.Message);
                        }
                        catch (AdminInactiveException ex)
                        {
                            context.Fail(ex.Message);
                        }
                    }
                };

                Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultMapInboundClaims = false;
            });

        services.AddAuthorization(auth =>
        {
            foreach (var permission in AdminPermissions.All)
            {
                auth.AddPolicy(permission, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(AdminClaimTypes.Permission, permission);
                });
            }
        });

        return services;
    }

    private static byte[]? TryFromBase64(string input)
    {
        try
        {
            return Convert.FromBase64String(input);
        }
        catch
        {
            return null;
        }
    }
}
