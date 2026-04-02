using System.Text.Json;
using DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;
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
    
    public string BuildVlessLink(
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
        var accessUrl = BuildPeerConfigAccessUrl(peerUuid);
        return $"happ://add/{accessUrl}";
    }
    
    public string BuildV2RayTunDeepLink(Guid peerUuid)
    {
        var accessUrl = BuildPeerConfigAccessUrl(peerUuid);
        return $"v2raytun://import/{Uri.EscapeDataString(accessUrl)}";
    }

    public string BuildPeerConfigAccessUrl(Guid peerUuid)
    {
        if (peerUuid == Guid.Empty)
            throw new ArgumentException("Peer UUID is required.", nameof(peerUuid));

        if (string.IsNullOrWhiteSpace(_linkOptions.PublicBaseUrl))
            throw new InvalidOperationException("VpnLinkOptions.PublicBaseUrl is required.");

        if (string.IsNullOrWhiteSpace(_linkOptions.PeerConfigPath))
            throw new InvalidOperationException("VpnLinkOptions.PeerConfigPath is required.");

        return
            $"{_linkOptions.PublicBaseUrl.TrimEnd('/')}/" +
            $"{_linkOptions.PeerConfigPath.TrimStart('/')}/{peerUuid}";
    }

    public string BuildXrayClientConfig(
    PeerDataForConfigDto peer,
    ServerConfigDataDto server)
    {
        var config = new XrayClientConfigDto
        {
            Dns = new DnsDto
            {
                Servers = new[] { "1.1.1.1", "8.8.8.8" }
            },
            Inbounds = new[]
            {
                new InboundDto
                {
                    Listen = "127.0.0.1",
                    Port = 10808,
                    Protocol = "socks",
                    Settings = new { auth = "noauth", udp = true },
                    Sniffing = new SniffingDto
                    {
                        DestOverride = new[] { "http", "tls", "quic" },
                        Enabled = true,
                        RouteOnly = true
                    },
                    Tag = "socks"
                },
                new InboundDto
                {
                    Listen = "127.0.0.1",
                    Port = 10809,
                    Protocol = "http",
                    Settings = new { allowTransparent = false },
                    Sniffing = new SniffingDto
                    {
                        DestOverride = new[] { "http", "tls", "quic" },
                        Enabled = true,
                        RouteOnly = true
                    },
                    Tag = "http"
                }
            },
            Outbounds = new[]
            {
                new OutboundDto
                {
                    Protocol = "vless",
                    Settings = new
                    {
                        vnext = new[]
                        {
                            new
                            {
                                address = server.PublicHost,
                                port = server.PublicPort,
                                users = new[]
                                {
                                    new
                                    {
                                        id = peer.AgentUuid,
                                        encryption = "none",
                                        flow = "xtls-rprx-vision"
                                    }
                                }
                            }
                        }
                    },
                    StreamSettings = new StreamSettingsDto
                    {
                        RealitySettings = new RealitySettingsDto
                        {
                            Fingerprint = "qq",
                            PublicKey = server.RealityPublicKey,
                            ServerName = server.RealitySni,
                            ShortId = server.RealityShortId,
                            SpiderX = "/"
                        },
                        TcpSettings = new TcpSettingsDto
                        {
                            Header = new HeaderDto()
                        }
                    },
                    Tag = "proxy"
                },
                new OutboundDto { Protocol = "freedom", Tag = "direct", Settings = new { } },
                new OutboundDto { Protocol = "blackhole", Tag = "block", Settings = new { } }
            },
            Remarks = server.Region,
            Routing = new RoutingDto()
        };

        return JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
} 