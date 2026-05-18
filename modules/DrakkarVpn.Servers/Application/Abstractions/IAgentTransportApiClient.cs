using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Servers.Application.Abstractions;

public interface IAgentTransportApiClient
{
    Task<AgentTransportApplyCallResult> ApplyServerTransportAsync(
        string agentBaseUrl,
        AgentApplyServerTransportRequest request,
        CancellationToken ct);
}
