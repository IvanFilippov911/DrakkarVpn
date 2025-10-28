using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.PeersMapByDeviceIdsOnServer;

public sealed class PeersMapByDeviceIdsOnServerHandler
    : IRequestHandler<PeersMapByDeviceIdsOnServerQuery, Dictionary<string, PeerBriefDto>>
{
    private readonly IPeerRepository _repo;

    public PeersMapByDeviceIdsOnServerHandler(IPeerRepository repo) => _repo = repo;

    public Task<Dictionary<string, PeerBriefDto>> Handle(
        PeersMapByDeviceIdsOnServerQuery q, CancellationToken ct)
        => _repo.GetMapByDeviceIdsOnServerAsync(q.ServerId, q.DeviceIds, ct);
}
