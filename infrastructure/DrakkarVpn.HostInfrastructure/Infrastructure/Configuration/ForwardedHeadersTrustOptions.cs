namespace DrakkarVpn.HostInfrastructure.Infrastructure.Configuration;

public sealed class ForwardedHeadersTrustOptions
{
    public const string SectionName = "ForwardedHeaders";

    public string[] KnownProxies { get; set; } = Array.Empty<string>();

    public string[] KnownNetworks { get; set; } = Array.Empty<string>();
}
