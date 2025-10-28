using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DrakkarVpn.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddOptions<JwtOptions>()
            .Bind(cfg.GetSection("Jwt"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.SigningKey), "Jwt:SigningKey is missing")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer),     "Jwt:Issuer is missing")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience),   "Jwt:Audience is missing")
            .Validate(o => o.DefaultTtlMinutes > 0,                  "Jwt:DefaultTtlMinutes must be > 0")
            .ValidateOnStart();
        
        var opts = cfg.GetSection("Jwt").Get<JwtOptions>()!;
        var keyBytes = TryFromBase64(opts.SigningKey) ?? Encoding.UTF8.GetBytes(opts.SigningKey);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 bytes (256-bit). Use Base64 string.");

        var securityKey = new SymmetricSecurityKey(keyBytes);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = opts.Issuer,
                    ValidateAudience = true,
                    ValidAudience = opts.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                
                Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultMapInboundClaims = false;
            });

        services.AddAuthorization();

        return services;
    }

    private static byte[]? TryFromBase64(string input)
    {
        try { return Convert.FromBase64String(input); } catch { return null; }
    }
}