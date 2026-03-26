using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Peers.Application.Services;

public sealed class VpnConfigBuilder : IVpnConfigBuilder
{
    public string Build(
        PeerDataForConfigDto peer,
        ServerConfigDataDto server)
    {
        return
            $"vless://{peer.AgentUuid}@{server.PublicHost}:{server.PublicPort}" +
            $"?security=reality" +
            $"&encryption=none" +
            $"&type=tcp" +
            $"&sni={Uri.EscapeDataString(server.RealitySni)}" +
            $"&pbk={Uri.EscapeDataString(server.RealityPublicKey)}" +
            $"&sid={Uri.EscapeDataString(server.RealityShortId)}" +
            $"&fp=chrome" +
            $"#DrakkarNetwork";
    }
} 