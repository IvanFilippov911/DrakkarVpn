using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IAgentApplyServerTransportContextRepository
{
    Task<IReadOnlyDictionary<Guid, AgentApplyServerTransportContext>> GetByActivationIdsAsync(
        IReadOnlyCollection<Guid> activationIds,
        CancellationToken ct);
}
