using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IAgentTransportStateRepository
{
    Task<AgentTransportStateDto?> GetCurrentAsync(CancellationToken ct);

    Task UpsertAppliedAsync(
        Guid serverId,
        Guid activationId,
        Guid operationId,
        string payloadHash,
        DateTime appliedAtUtc,
        CancellationToken ct);
}
