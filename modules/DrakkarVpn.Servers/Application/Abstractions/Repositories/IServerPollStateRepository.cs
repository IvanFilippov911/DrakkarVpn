using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerPollStateRepository
{
    Task<IReadOnlyList<ServerPollCandidateDto>> AcquireBatchAsync(
        DateTime nowUtc,
        int batchSize,
        TimeSpan leaseDuration,
        TimeSpan stuckTimeout,
        string instanceId,
        CancellationToken ct);

    Task<ApplyPollStateResultDto> ApplyResultsAsync(
        IReadOnlyCollection<ServerPollResultDto> results,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct);
}