using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IAgentTransportApplyService
{
    Task<AgentTransportApplyResult> ApplyAsync(ApplyServerTransportRequestDto request, CancellationToken ct);
}
