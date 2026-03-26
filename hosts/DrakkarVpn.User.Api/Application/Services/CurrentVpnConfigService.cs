using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Core.Api.Application.Services;

public sealed class CurrentVpnConfigService : ICurrentVpnConfigService
{
    private readonly IPeersQueryService _peers;
    private readonly IServersQueryService _servers;
    private readonly IVpnConfigBuilder _builder;
    private readonly VpnLinkOptions _link;

    public CurrentVpnConfigService(
        IPeersQueryService peers,
        IServersQueryService servers,
        IVpnConfigBuilder builder,
        IOptions<VpnLinkOptions> linkOptions)
    {
        _peers = peers;
        _servers = servers;
        _builder = builder;
        _link = linkOptions.Value;
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

        var configRaw = _builder.Build(peer, server);

        return new CurrentVpnConfigDto(
            ConfigRaw: configRaw,
            HappLink: _link.BuildHappLink(peer.AgentUuid.ToString()));
    }
}