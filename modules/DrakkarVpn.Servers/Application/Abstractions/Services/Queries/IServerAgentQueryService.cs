using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

public interface IServerAgentQueryService : IServerQueryForPeers
{
    Task<ServerForAgentDto?> GetServerForAgentAsync(Guid serverId, CancellationToken ct);
}
