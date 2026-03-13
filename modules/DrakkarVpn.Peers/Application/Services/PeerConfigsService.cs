using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerConfigsService : IPeerConfigsService
{
    private readonly IPeerRepository _peers;

    public PeerConfigsService(IPeerRepository peers)
    {
        _peers = peers;
    }

    public async Task<PeerConfigDto?> GetByAgentUuidAsync(Guid peerUuid, CancellationToken ct)
    {
        var peer = await _peers.GetByAgentUuidAsync(peerUuid, ct);
        if (peer is null) return null;

        return new PeerConfigDto(peer.ConfigRaw);
    }
}