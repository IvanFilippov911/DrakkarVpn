namespace DrakkarVpn.Agent.Infrastructure.Config;

public sealed class V2RayOptions
{
    public string ServerHost { get; set; } = "127.0.0.1";
    public int VlessPort { get; set; } = 443;
    public int ApiPort { get; set; }
}