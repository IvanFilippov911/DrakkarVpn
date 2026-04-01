namespace DrakkarVpn.Agent.Infrastructure.Config;

public sealed class XrayOptions
{
    public string PublicHost { get; set; } = "vpn.example.com";
    public int PublicPort { get; set; } = 443;

    public string LocalApiHost { get; set; } = "127.0.0.1";
    public int ApiPort { get; set; } = 10085;

    public string InboundTag { get; set; } = "vless-reality-in";
    public string UserEmailSuffix { get; set; } = "@drakkar.local";
    public string VlessFlow { get; set; } = "xtls-rprx-vision";
    public string VlessEncryption { get; set; } = "none";

    public string DefaultInterface { get; set; } = "eth0";
    public string InfraProbeUrl { get; set; } = "https://google.com";
}
