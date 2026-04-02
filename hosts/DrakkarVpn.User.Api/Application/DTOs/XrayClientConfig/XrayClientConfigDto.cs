namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class XrayClientConfigDto
{
    public DnsDto Dns { get; init; } = default!;
    public IReadOnlyList<InboundDto> Inbounds { get; init; } = default!;
    public IReadOnlyList<OutboundDto> Outbounds { get; init; } = default!;
    public string Remarks { get; init; } = default!;
    public RoutingDto Routing { get; init; } = default!;
}