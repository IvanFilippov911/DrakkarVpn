using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Servers.Application.DTOs.ServerState;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerPollResultApplyRepository
{
    Task<IReadOnlyDictionary<Guid, ServerState>> GetStateAsync(
        IReadOnlyCollection<Guid> serverIds,
        CancellationToken ct);

    Task<IReadOnlyList<ServerInfoUpdateDto>> ApplyUpdatesAsync(
        IReadOnlyCollection<ServerUpdate> updates,
        CancellationToken ct);
}