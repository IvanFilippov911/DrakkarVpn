namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class SniffingDto
{
    public IReadOnlyList<string> DestOverride { get; init; } = default!;
    public bool Enabled { get; init; }
    public bool RouteOnly { get; init; }
}