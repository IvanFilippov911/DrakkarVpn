namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class StreamSettingsDto
{
    public string Network { get; init; } = "tcp";
    public string Security { get; init; } = "reality";
    public RealitySettingsDto RealitySettings { get; init; } = default!;
    public TcpSettingsDto TcpSettings { get; init; } = default!;
}