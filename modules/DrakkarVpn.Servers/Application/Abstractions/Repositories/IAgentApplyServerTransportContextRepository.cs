using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Repositories;

public interface IAgentApplyServerTransportContextRepository
{
    Task<IReadOnlyDictionary<Guid, AgentApplyServerTransportContext>> GetByActivationIdsAsync(
        IReadOnlyCollection<Guid> activationIds,
        CancellationToken ct);
}
