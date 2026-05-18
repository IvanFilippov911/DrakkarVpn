using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

namespace DrakkarVpn.Servers.Application.Services.Queries;

public sealed class ServerAgentQueryService : IServerAgentQueryService
{
    private readonly IServerRepository _servers;

    public ServerAgentQueryService(IServerRepository servers)
        => _servers = servers;

    public Task<ServerShortDto?> GetShortAsync(Guid serverId, CancellationToken ct)
        => _servers.GetServerShortAsync(serverId, ct);

    public Task<ServerForAgentDto?> GetServerForAgentAsync(Guid serverId, CancellationToken ct)
        => _servers.GetForAgentAsync(serverId, ct);

    public async Task<IReadOnlyDictionary<Guid, string>> GetAgentBaseUrlsByIdsAsync(
        IReadOnlyCollection<Guid> serverIds,
        CancellationToken ct)
    {
        if (serverIds.Count == 0)
            return new Dictionary<Guid, string>();

        var serversById = await _servers.GetByIdsAsync(serverIds.ToArray(), ct);

        return serversById.ToDictionary(
            pair => pair.Key,
            pair => NormalizeBaseUrl(pair.Value.AgentBaseUrl));
    }

    public Task<string?> GetAgentBaseUrlAsync(Guid serverId, CancellationToken ct)
        => _servers.GetAgentBaseUrlAsync(serverId, ct);

    private static string NormalizeBaseUrl(Uri uri)
    {
        var s = uri.AbsoluteUri.TrimEnd('/');
        return s + "/";
    }
}
