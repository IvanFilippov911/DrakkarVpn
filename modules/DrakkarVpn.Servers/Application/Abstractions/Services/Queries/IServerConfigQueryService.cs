using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

public interface IServerConfigQueryService
{
    Task<ServerConfigDataDto?> GetDataForConfigByIdAsync(Guid serverId, CancellationToken ct);

    Task<Guid[]> GetEnabledServerIdsAsync(CancellationToken ct);
}
