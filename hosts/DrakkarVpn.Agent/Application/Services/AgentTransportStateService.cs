using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class AgentTransportStateService : IAgentTransportStateService
{
    private readonly IAgentTransportStateRepository _repository;

    public AgentTransportStateService(IAgentTransportStateRepository repository)
    {
        _repository = repository;
    }

    public Task<AgentTransportStateDto?> GetCurrentAsync(CancellationToken ct) =>
        _repository.GetCurrentAsync(ct);

    public Task MarkAppliedAsync(
        Guid serverId,
        Guid activationId,
        Guid operationId,
        string payloadHash,
        DateTime appliedAtUtc,
        CancellationToken ct) =>
        _repository.UpsertAppliedAsync(serverId, activationId, operationId, payloadHash, appliedAtUtc, ct);
}
