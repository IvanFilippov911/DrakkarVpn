using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;

public sealed class GetPeerByUuidHandler
    : IRequestHandler<GetPeerByUuidQuery, PeerConfigDto?>
{
    private readonly IPeerConfigsService _svc;

    public GetPeerByUuidHandler(IPeerConfigsService svc) => _svc = svc;

    public async Task<PeerConfigDto?> Handle(GetPeerByUuidQuery q, CancellationToken ct)
    {
        var peer = await _svc.GetByAgentUuidAsync(q.PeerUuid, ct);
        if (peer is null) return null;

        return new PeerConfigDto(peer.ConfigRaw);
    }
}