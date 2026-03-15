namespace DrakkarVpn.HostInfrastructure.Infrastructure.Configuration;

public sealed class FrontendCorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
