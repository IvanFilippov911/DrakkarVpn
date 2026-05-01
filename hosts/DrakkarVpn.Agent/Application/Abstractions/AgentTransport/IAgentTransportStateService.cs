using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IAgentTransportStateService
{
    Task<AgentTransportStateDto?> GetCurrentAsync(CancellationToken ct);

    Task MarkAppliedAsync(
        Guid serverId,
        Guid activationId,
        Guid operationId,
        string payloadHash,
        DateTime appliedAtUtc,
        CancellationToken ct);
}
