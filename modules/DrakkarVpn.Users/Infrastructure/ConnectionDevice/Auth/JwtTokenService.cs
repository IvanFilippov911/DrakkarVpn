using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _defaultTtl;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        var o = options.Value;
        var keyBytes = TryFromBase64(o.SigningKey) ?? Encoding.UTF8.GetBytes(o.SigningKey);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 bytes (256-bit). Use Base64 string.");

        _key = new SymmetricSecurityKey(keyBytes);
        _issuer = o.Issuer;
        _audience = o.Audience;
        _defaultTtl = TimeSpan.FromMinutes(o.DefaultTtlMinutes);
        
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    }

    public string IssueToken(
        long telegramId,
        string deviceId,
        TimeSpan? ttl = null,
        string? platform = null,
        string? deviceName = null)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new("telegram_id", telegramId.ToString()),
            new("device_id", deviceId),
            new(JwtRegisteredClaimNames.Iat, ToUnix(now).ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrWhiteSpace(platform))
            claims.Add(new("platform", platform));
        if (!string.IsNullOrWhiteSpace(deviceName))
            claims.Add(new("device_name", deviceName));
        
        claims.Add(new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(ttl ?? _defaultTtl),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static long ToUnix(DateTime utc) =>
        (long)Math.Floor((utc - DateTime.UnixEpoch).TotalSeconds);

    private static byte[]? TryFromBase64(string input)
    {
        try { return Convert.FromBase64String(input); } catch { return null; }
    }
}

