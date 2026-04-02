using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

namespace DrakkarVpn.Core.Api.Application.Services;

public sealed class CurrentVpnConfigService : ICurrentVpnConfigService
{
    private readonly IPeersQueryService _peers;
    private readonly IServersQueryService _servers;
    private readonly IVpnBuildArtifactsService _builder;

    public CurrentVpnConfigService(
        IPeersQueryService peers,
        IServersQueryService servers,
        IVpnBuildArtifactsService builder)
    {
        _peers = peers;
        _servers = servers;
        _builder = builder;
    }

    public async Task<CurrentVpnConfigDto?> GetCurrentConfigAsync(
        VpnAccessContextDto access,
        CancellationToken ct)
    {
        var peer = await _peers.GetDataForConfigByDeviceIdAsync(access.DeviceId, ct);
        if (peer is null)
            return null;

        var server = await _servers.GetDataForConfigByIdAsync(peer.ServerId, ct);
        if (server is null)
            throw new InvalidOperationException($"Server '{peer.ServerId}' not found.");

        var configRaw = _builder.BuildXrayClientConfig(peer, server);
        var configVless = _builder.BuildVlessLink(peer, server);
        var configUrl = _builder.BuildPeerConfigAccessUrl(peer.AgentUuid);
        var happLink = _builder.BuildHappLink(peer.AgentUuid);
        var v2RayLink = _builder.BuildV2RayTunDeepLink(peer.AgentUuid);

        return new CurrentVpnConfigDto(
            ConfigRaw: configRaw,
            ConfigVless: configVless,
            ConfigUrl: configUrl,
            HappLink: happLink,
            V2rayLink: v2RayLink);
    }
}