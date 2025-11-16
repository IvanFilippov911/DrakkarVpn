using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.CountPeersOnServer;

public sealed class CountPeersOnServerHandler
    : IRequestHandler<CountPeersOnServerQuery, int>
{
    private readonly IPeerRepository _peers;

    public CountPeersOnServerHandler(IPeerRepository peers)
    {
        _peers = peers;
    }

    public Task<int> Handle(CountPeersOnServerQuery q, CancellationToken ct)
        => _peers.CountOnServerAsync(q.ServerId, ct);
}