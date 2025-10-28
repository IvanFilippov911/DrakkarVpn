using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.PeersMapByDeviceIdsOnServer;

public sealed record PeersMapByDeviceIdsOnServerQuery(
    Guid ServerId,
    IReadOnlyCollection<string> DeviceIds
) : IRequest<Dictionary<string, PeerBriefDto>>;