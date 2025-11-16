using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetServerPeersOnlineHistory;

public sealed record GetServerPeersOnlineHistoryQuery(
    Guid ServerId,
    int Minutes = 24 * 60
) : IRequest<IReadOnlyList<ServerOnlinePointDto>>;