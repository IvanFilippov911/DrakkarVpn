namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;

public sealed class VpnLinkOptions
{
    public string PublicBaseUrl { get; init; } = string.Empty;
    public string PeerConfigPath { get; init; } = string.Empty;
    
    public string BuildHappLink(string peerUuid)
    {
        return $"happ://add/{PublicBaseUrl.TrimEnd('/')}/{PeerConfigPath.TrimStart('/')}/{peerUuid}";
    }
}