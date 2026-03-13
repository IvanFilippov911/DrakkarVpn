using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

namespace DrakkarVpn.Shared.Servers;

public interface IServerQueryForPeers
{
    Task<ServerShortDto?> GetShortAsync(Guid serverId, CancellationToken ct);

    Task<IReadOnlyDictionary<Guid, string>> GetAgentBaseUrlsByIdsAsync(
        IReadOnlyCollection<Guid> serverIds,
        CancellationToken ct);
    Task<string?> GetAgentBaseUrlAsync(Guid serverId, CancellationToken ct);
}