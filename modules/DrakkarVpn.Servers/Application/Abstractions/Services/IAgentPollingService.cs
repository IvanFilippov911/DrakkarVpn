using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Servers.Application.DTOs.ServerState;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IAgentPollingService
{
    Task<IReadOnlyList<ServerPollResultDto>> PollBatchAsync(
        IReadOnlyList<ServerPollCandidateDto> candidates,
        int httpConcurrency,
        CancellationToken ct);
}