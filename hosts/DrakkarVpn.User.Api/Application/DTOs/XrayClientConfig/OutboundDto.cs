namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class OutboundDto
{
    public string Protocol { get; init; } = default!;
    public object Settings { get; init; } = default!;
    public StreamSettingsDto? StreamSettings { get; init; }
    public string Tag { get; init; } = default!;
}