namespace DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;

public sealed class RoutingDto
{
    public string DomainMatcher { get; init; } = "hybrid";
    public string DomainStrategy { get; init; } = "IPIfNonMatch";
    public IReadOnlyList<object> Rules { get; init; } = Array.Empty<object>();
}