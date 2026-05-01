namespace DrakkarVpn.Agent.Infrastructure.EF.Entities;

public sealed class AgentTransportState
{
    private AgentTransportState() { }

    public int Id { get; private set; } = 1;

    public Guid ServerId { get; private set; }
    public Guid ActivationId { get; private set; }
    public Guid OperationId { get; private set; }

    public string PayloadHash { get; private set; } = default!;

    public DateTime AppliedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static AgentTransportState Create(
        Guid serverId,
        Guid activationId,
        Guid operationId,
        string payloadHash,
        DateTime utcNow)
    {
        return new AgentTransportState
        {
            Id = 1,
            ServerId = serverId,
            ActivationId = activationId,
            OperationId = operationId,
            PayloadHash = payloadHash,
            AppliedAtUtc = utcNow,
            UpdatedAtUtc = utcNow
        };
    }

    public void MarkApplied(
        Guid serverId,
        Guid activationId,
        Guid operationId,
        string payloadHash,
        DateTime utcNow)
    {
        ServerId = serverId;
        ActivationId = activationId;
        OperationId = operationId;
        PayloadHash = payloadHash;
        AppliedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }
}