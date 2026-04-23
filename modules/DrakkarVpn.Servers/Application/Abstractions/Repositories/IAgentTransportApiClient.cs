using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IAgentTransportApiClient
{
    Task ApplyServerTransportAsync(
        string agentBaseUrl,
        AgentApplyServerTransportRequest request,
        CancellationToken ct);
}