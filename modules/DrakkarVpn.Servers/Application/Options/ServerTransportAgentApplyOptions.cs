namespace DrakkarVpn.Servers.Application.Options;

public sealed class ServerTransportAgentApplyOptions
{
    public const string SectionName = "ServerTransportAgentApply";

    public int MaxParallelRequests { get; init; } = 50;

    public string InboundTag { get; init; } = "vless-reality-in";
    public string Flow { get; init; } = "xtls-rprx-vision";
    public string Encryption { get; init; } = "none";
}
