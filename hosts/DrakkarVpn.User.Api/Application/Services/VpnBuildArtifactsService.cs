using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Core.Api.Application.Services;

public sealed class VpnBuildArtifactsService : IVpnBuildArtifactsService
{
    private readonly VpnLinkOptions _linkOptions;

    public VpnBuildArtifactsService(IOptions<VpnLinkOptions> linkOptions)
    {
        _linkOptions = linkOptions.Value;
    }
    
    public string BuildConfig(
        PeerDataForConfigDto peer,
        ServerConfigDataDto server)
    {
        return
            $"vless://{peer.AgentUuid}@{server.PublicHost}:{server.PublicPort}" +
            $"?security=reality" +
            $"&flow=xtls-rprx-vision" +
            $"&encryption=none" +
            $"&type=tcp" +
            $"&sni={Uri.EscapeDataString(server.RealitySni)}" +
            $"&pbk={Uri.EscapeDataString(server.RealityPublicKey)}" +
            $"&sid={Uri.EscapeDataString(server.RealityShortId)}" +
            $"&fp=chrome" +
            $"#DrakkarNetwork";
    }
    
    public string BuildHappLink(Guid peerUuid)
    {
        if (peerUuid == Guid.Empty)
            throw new ArgumentException("Peer UUID is required.", nameof(peerUuid));

        if (string.IsNullOrWhiteSpace(_linkOptions.PublicBaseUrl))
            throw new InvalidOperationException("VpnLinkOptions.PublicBaseUrl is required.");

        if (string.IsNullOrWhiteSpace(_linkOptions.PeerConfigPath))
            throw new InvalidOperationException("VpnLinkOptions.PeerConfigPath is required.");
        
        var accessUrl = BuildPeerAccessUrl(peerUuid);
        return $"happ://add/{accessUrl}";
    }
    
    public string BuildV2RayTunDeepLink(Guid peerUuid)
    {
        var accessUrl = BuildPeerAccessUrl(peerUuid);
        return $"v2raytun://import/{Uri.EscapeDataString(accessUrl)}";
    }

    private string BuildPeerAccessUrl(Guid peerUuid)
    {
        return
            $"{_linkOptions.PublicBaseUrl.TrimEnd('/')}/" +
            $"{_linkOptions.PeerConfigPath.TrimStart('/')}/{peerUuid}";
    }
} 