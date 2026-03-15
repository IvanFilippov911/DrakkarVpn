namespace DrakkarVpn.AdminAuth.Application.Options;

public sealed class AdminJwtOptions
{
    public const string SectionName = "AdminJwt";

    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenTtlMinutes { get; set; } = 15;
}
