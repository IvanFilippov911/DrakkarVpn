namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class GrpcSettingsDto
{
    public string ServiceName { get; init; } = default!;
    public string? Authority { get; init; }
}