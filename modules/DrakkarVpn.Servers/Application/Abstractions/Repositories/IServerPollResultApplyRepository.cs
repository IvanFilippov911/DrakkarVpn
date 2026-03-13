using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Observability.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerPollResultApplyRepository
{
    Task<IReadOnlyList<ServerInfoUpdateDto>> ApplyBatchAsync(
        IReadOnlyCollection<ServerPollResultDto> appliedResults,
        DateTime nowUtc,
        CancellationToken ct);
}