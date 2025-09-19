using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetActivePeersCount;

public sealed class GetActivePeersCountHandler : IRequestHandler<GetActivePeersCountRequest, int>
{
    private readonly IPeerRepository _peers;

    public GetActivePeersCountHandler(IPeerRepository peers) => _peers = peers;

    public async Task<int> Handle(GetActivePeersCountRequest req, CancellationToken ct)
    {
        return await _peers.GetActiveCountByServerIdAsync(req.ServerId, ct);
    }
}