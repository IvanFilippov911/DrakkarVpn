using System.Text.Json;
using DrakkarVpn.Core.Api.Application.DTOs.XrayClientConfig;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Servers.Domain.Enums;
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
        EnsureSupportedSecurity(server);
        var fp = NormalizeFingerprint(server.RealityFingerprint);

        var queryParts = new List<string>
        {
            "security=reality",
            "flow=xtls-rprx-vision",
            "encryption=none",
            $"type={Uri.EscapeDataString(MapTransportType(server.TransportType))}",
            $"sni={Uri.EscapeDataString(server.RealitySni)}",
            $"pbk={Uri.EscapeDataString(server.RealityPublicKey)}",
            $"sid={Uri.EscapeDataString(server.RealityShortId)}",
            $"fp={Uri.EscapeDataString(fp)}"
        };

        if (server.TransportType == TransportType.Grpc)
        {
            if (string.IsNullOrWhiteSpace(server.GrpcServiceName))
                throw new InvalidOperationException("GrpcServiceName is required for gRPC transport.");

            queryParts.Add($"serviceName={Uri.EscapeDataString(server.GrpcServiceName)}");

            if (!string.IsNullOrWhiteSpace(server.GrpcAuthority))
                queryParts.Add($"authority={Uri.EscapeDataString(server.GrpcAuthority)}");
        }

        var query = string.Join("&", queryParts);

        return
            $"vless://{peer.AgentUuid}@{server.PublicHost}:{server.PublicPort}" +
            $"?{query}" +
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
        EnsureSupportedSecurity(server);
        var fp = NormalizeFingerprint(server.RealityFingerprint);

        var streamSettings = BuildStreamSettings(server, fp);

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
                    StreamSettings = streamSettings,
                    Tag = "proxy"
                },
                new OutboundDto
                {
                    Protocol = "freedom",
                    Tag = "direct",
                    Settings = new { }
                },
                new OutboundDto
                {
                    Protocol = "blackhole",
                    Tag = "block",
                    Settings = new { }
                }
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

    private static StreamSettingsDto BuildStreamSettings(ServerConfigDataDto server, string fingerprint)
    {
        var realitySettings = new RealitySettingsDto
        {
            Fingerprint = fingerprint,
            PublicKey = server.RealityPublicKey,
            ServerName = server.RealitySni,
            ShortId = server.RealityShortId,
            SpiderX = "/"
        };

        return server.TransportType switch
        {
            TransportType.Tcp => new StreamSettingsDto
            {
                Network = "tcp",
                Security = "reality",
                RealitySettings = realitySettings,
                TcpSettings = new TcpSettingsDto
                {
                    Header = new HeaderDto()
                }
            },

            TransportType.Grpc => new StreamSettingsDto
            {
                Network = "grpc",
                Security = "reality",
                RealitySettings = realitySettings,
                GrpcSettings = new GrpcSettingsDto
                {
                    ServiceName = !string.IsNullOrWhiteSpace(server.GrpcServiceName)
                        ? server.GrpcServiceName
                        : throw new InvalidOperationException("GrpcServiceName is required for gRPC transport."),
                    Authority = server.GrpcAuthority
                }
            },

            _ => throw new InvalidOperationException(
                $"Transport type '{server.TransportType}' is not supported for VPN config building.")
        };
    }

    private static void EnsureSupportedSecurity(ServerConfigDataDto server)
    {
        if (server.SecurityType != SecurityType.Reality)
        {
            throw new InvalidOperationException(
                $"Security type '{server.SecurityType}' is not supported for VPN config building.");
        }
    }

    private static string NormalizeFingerprint(string? fingerprint)
    {
        return string.IsNullOrWhiteSpace(fingerprint)
            ? "chrome"
            : fingerprint.Trim();
    }

    private static string MapTransportType(TransportType transportType)
    {
        return transportType switch
        {
            TransportType.Tcp => "tcp",
            TransportType.Grpc => "grpc",
            _ => throw new InvalidOperationException(
                $"Transport type '{transportType}' is not supported for VLESS link building.")
        };
    }
}