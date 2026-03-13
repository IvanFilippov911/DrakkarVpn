using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerPeersOnlineHistory;

public sealed record GetServerPeersOnlineHistoryQuery(
    Guid ServerId,
    int Minutes
) : IRequest<IReadOnlyList<ServerOnlinePointDto>>;