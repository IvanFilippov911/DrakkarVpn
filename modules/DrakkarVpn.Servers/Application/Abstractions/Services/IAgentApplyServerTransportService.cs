using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IAgentApplyServerTransportService
{
    Task<AgentApplyServerTransportWireResponse> ApplyAsync(Guid serverId, Guid activationId, CancellationToken ct);
}