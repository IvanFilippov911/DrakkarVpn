namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Options;

public sealed class VlessLinkOptions
{
    public int PublicPort { get; init; } = 443;
    
    public string TagPrefix { get; init; } = "Drakkar";
    public bool AllowInsecure { get; init; } = true;
}