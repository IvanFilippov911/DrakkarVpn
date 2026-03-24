using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Services;

public sealed class CurrentVpnConfigService : ICurrentVpnConfigService
{
    private readonly IPeersQueryService _peers;
    private readonly VpnLinkOptions _link;

    public CurrentVpnConfigService(
        IPeersQueryService peers,
        IOptions<VpnLinkOptions> linkOptions)
    {
        _peers = peers;
        _link = linkOptions.Value;
    }

    public async Task<CurrentVpnConfigDto?> GetCurrentConfigAsync(
        VpnAccessContextDto access,
        CancellationToken ct)
    {
        var peer = await _peers.GetPeerByDeviceAsync(access.DeviceId, ct);
        if (peer is null) return null;

        return new CurrentVpnConfigDto(
            ConfigRaw: peer.ConfigRaw,
            HappLink: _link.BuildHappLink(peer.AgentPeerId.ToString()));
    }
}

