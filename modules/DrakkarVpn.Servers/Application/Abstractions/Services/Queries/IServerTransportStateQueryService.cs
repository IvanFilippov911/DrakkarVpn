using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

public interface IServerTransportStateQueryService
{
    Task<Dictionary<Guid, ServerTransportDesiredStateDto>> GetTransportDesiredStatesAsync(
        Guid[] serverIds,
        CancellationToken ct);
}
