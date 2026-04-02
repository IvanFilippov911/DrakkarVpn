namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class RealitySettingsDto
{
    public string Fingerprint { get; init; } = "chrome";
    public string PublicKey { get; init; } = default!;
    public string ServerName { get; init; } = default!;
    public string ShortId { get; init; } = default!;
    public string SpiderX { get; init; } = "/";
}