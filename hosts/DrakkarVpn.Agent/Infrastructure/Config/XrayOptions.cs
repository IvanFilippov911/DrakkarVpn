namespace DrakkarVpn.Agent.Infrastructure.Config;

public sealed class XrayOptions
{
    public string PublicHost { get; set; } = "vpn.example.com";
    public int PublicPort { get; set; } = 443;

    public string LocalApiHost { get; set; } = "127.0.0.1";
    public int ApiPort { get; set; } = 10085;

    public string InboundTag { get; set; } = "vless-in";
    public string UserEmailSuffix { get; set; } = "@drakkar.local";

    public string DefaultInterface { get; set; } = "eth0";
    public string InfraProbeUrl { get; set; } = "https://google.com";
}
