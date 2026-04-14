using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Servers.Application.DTOs.ServerState;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerPollResultApplyService
{
    Task ApplyPollResultsAsync(
        IReadOnlyCollection<ServerPollResultDto> appliedResults,
        DateTime nowUtc,
        CancellationToken ct);
}