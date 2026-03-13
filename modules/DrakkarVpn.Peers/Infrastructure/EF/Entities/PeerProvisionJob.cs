using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

public sealed class PeerProvisionJob
{
    public Guid JobId { get; private set; }
    public Guid UserId { get; private set; }
    public string DeviceId { get; private set; } = default!;
    public Guid ServerId { get; private set; }
    
    public DateTime? AgentAppliedAtUtc { get; private set; }

    public PeerProvisionState State { get; private set; }
    public int Attempt { get; private set; }
    public int MaxAttempts { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime NextAttemptAtUtc { get; private set; }
    
    public string? LeaseOwner { get; private set; }
    public DateTime? LeaseUntilUtc { get; private set; }
    
    public Guid? PeerId { get; private set; }
    public Guid? AgentPeerUuid { get; private set; }
    public string? ConfigRaw { get; private set; }

    public string? LastErrorCode { get; private set; }
    public string? LastErrorMessage { get; private set; }
    
    private PeerProvisionJob() { } 

    internal PeerProvisionJob( 
        Guid jobId,
        Guid userId,
        string deviceId,
        Guid serverId,
        PeerProvisionState state,
        int attempt,
        int maxAttempts,
        DateTime createdAtUtc,
        DateTime updatedAtUtc,
        DateTime nextAttemptAtUtc)
    {
        JobId = jobId;
        UserId = userId;
        DeviceId = deviceId;
        ServerId = serverId;
        State = state;
        Attempt = attempt;
        MaxAttempts = maxAttempts;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        NextAttemptAtUtc = nextAttemptAtUtc;
    }
    
    public static PeerProvisionJob CreatePrepared(
        Guid userId,
        string deviceId,
        Guid serverId,
        Guid agentPeerUuid,
        string configRaw,
        int maxAttempts,
        DateTime nowUtc)
    {

        return new PeerProvisionJob(
            jobId: Guid.NewGuid(),
            userId: userId,
            deviceId: deviceId,
            serverId: serverId,
            state: PeerProvisionState.Pending,
            attempt: 0,
            maxAttempts: maxAttempts,
            createdAtUtc: nowUtc,
            updatedAtUtc: nowUtc,
            nextAttemptAtUtc: nowUtc
        )
        {
            AgentPeerUuid = agentPeerUuid,
            ConfigRaw     = configRaw
        };
    }
}