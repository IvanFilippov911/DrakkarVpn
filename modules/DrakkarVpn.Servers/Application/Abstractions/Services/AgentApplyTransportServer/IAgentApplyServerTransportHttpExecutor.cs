using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IAgentApplyServerTransportHttpExecutor
{
    Task<IReadOnlyDictionary<Guid, AgentApplyResult>> ExecuteAsync(
        IReadOnlyList<PreparedAgentTransportApplyRequest> requests,
        CancellationToken ct);
}
