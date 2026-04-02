namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class DnsDto
{
    public string QueryStrategy { get; init; } = "UseIPv4";
    public IReadOnlyList<string> Servers { get; init; } = default!;
}