namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class InboundDto
{
    public string Listen { get; init; } = "127.0.0.1";
    public int Port { get; init; }
    public string Protocol { get; init; } = default!;
    public object Settings { get; init; } = default!;
    public SniffingDto Sniffing { get; init; } = default!;
    public string Tag { get; init; } = default!;
}