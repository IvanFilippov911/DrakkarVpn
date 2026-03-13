using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Queries.Peers;

public sealed class PeersHandler 
    : IRequestHandler<PeersQuery, IReadOnlyList<PeersResultDto>>
{
    private readonly IXrayPeerClient _xray;

    public PeersHandler(IXrayPeerClient xray)
    {
        _xray = xray;
    }

    public Task<IReadOnlyList<PeersResultDto>> Handle(PeersQuery request, CancellationToken ct)
        => _xray.GetListPeersAsync(ct);
}