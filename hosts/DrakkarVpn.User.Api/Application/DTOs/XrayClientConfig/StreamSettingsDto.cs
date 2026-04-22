namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;



public sealed class StreamSettingsDto
{
    public string Network { get; init; } = default!;
    public string Security { get; init; } = default!;
    public RealitySettingsDto RealitySettings { get; init; } = default!;
    public TcpSettingsDto? TcpSettings { get; init; }
    public GrpcSettingsDto? GrpcSettings { get; init; }

}