using System.Net;
using System.Text.Json.Serialization;
using DrakkarVpn.HostInfrastructure.Infrastructure.Configuration;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace DrakkarVpn.Core.Api.Extensions;

public static class HostServiceCollectionExtensions
{
    public static IServiceCollection AddUserHostApi(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Drakkar User API", Version = "v1" }));

        return services;
    }

    public static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var section = configuration.GetSection(FrontendCorsOptions.SectionName);
        var corsOptions = section.Get<FrontendCorsOptions>() ?? new FrontendCorsOptions();

        services.AddOptions<FrontendCorsOptions>()
            .Bind(section)
            .Validate(
                o => o.AllowedOrigins.All(IsValidOrigin),
                $"{FrontendCorsOptions.SectionName}:AllowedOrigins must contain absolute HTTP(S) origins.")
            .Validate(
                o => !environment.IsProduction() || o.AllowedOrigins.Length > 0,
                $"{FrontendCorsOptions.SectionName}:AllowedOrigins must be configured in production.")
            .ValidateOnStart();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                if (corsOptions.AllowedOrigins.Length > 0)
                    policy.WithOrigins(corsOptions.AllowedOrigins);

                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddForwardedHeadersSupport(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var section = configuration.GetSection(ForwardedHeadersTrustOptions.SectionName);
        var trustOptions = section.Get<ForwardedHeadersTrustOptions>() ?? new ForwardedHeadersTrustOptions();

        services.AddOptions<ForwardedHeadersTrustOptions>()
            .Bind(section)
            .Validate(
                o => o.KnownProxies.All(IsValidIpAddress),
                $"{ForwardedHeadersTrustOptions.SectionName}:KnownProxies must contain valid IP addresses.")
            .Validate(
                o => o.KnownNetworks.All(IsValidCidr),
                $"{ForwardedHeadersTrustOptions.SectionName}:KnownNetworks must contain valid CIDR ranges.")
            .Validate(
                o => !environment.IsProduction() || o.KnownProxies.Length > 0 || o.KnownNetworks.Length > 0,
                $"{ForwardedHeadersTrustOptions.SectionName} must configure at least one trusted proxy or network in production.")
            .ValidateOnStart();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 1;

            if (trustOptions.KnownProxies.Length == 0 && trustOptions.KnownNetworks.Length == 0)
                return;

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            foreach (var proxy in trustOptions.KnownProxies)
                options.KnownProxies.Add(IPAddress.Parse(proxy));

            foreach (var network in trustOptions.KnownNetworks)
                options.KnownNetworks.Add(ParseNetwork(network));
        });

        return services;
    }

    private static bool IsValidOrigin(string origin)
    {
        return Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
               !string.IsNullOrWhiteSpace(uri.Host) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static bool IsValidIpAddress(string ipAddress)
        => IPAddress.TryParse(ipAddress, out _);

    private static bool IsValidCidr(string cidr)
    {
        var parts = cidr.Split('/', 2, StringSplitOptions.TrimEntries);

        if (parts.Length != 2 ||
            !IPAddress.TryParse(parts[0], out var prefix) ||
            !int.TryParse(parts[1], out var prefixLength))
        {
            return false;
        }

        var maxPrefixLength = prefix.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;
        return prefixLength is >= 0 && prefixLength <= maxPrefixLength;
    }

    private static IPNetwork ParseNetwork(string cidr)
    {
        var parts = cidr.Split('/', 2, StringSplitOptions.TrimEntries);
        return new IPNetwork(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static IServiceCollection AddSwaggerJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'"
            };

            c.AddSecurityDefinition("Bearer", bearerScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
    
    public static IServiceCollection AddAutoGeneratedLinks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VpnLinkOptions>(configuration.GetSection("VpnLink"));
        return services;
    }
}