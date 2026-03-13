namespace DrakkarVpn.Shared.Options;

public sealed class JwtOptions
{
    public string SigningKey { get; init; } = default!;
    public string Issuer     { get; init; } = default!;
    public string Audience   { get; init; } = default!;
    public int DefaultTtlMinutes { get; init; } = 60 * 24 * 30; // 30 дней
}