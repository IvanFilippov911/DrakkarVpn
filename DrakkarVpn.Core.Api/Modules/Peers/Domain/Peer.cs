using DrakkarVpn.Core.Api.Infrastructure.Absractions;

namespace DrakkarVpn.Core.Api.Modules.Peers.Domain;

public sealed class Peer : IAggregateRoot
{
    private Peer() { } 

    private Peer(
        PeerId id,
        Guid userId,
        Guid serverId,
        AgentPeerUuid agentPeerId,
        string configRaw,
        DateTime createdAt,
        DateTime? expiresAt)
    {
        Id = id;
        UserId = userId;
        ServerId = serverId;
        AgentPeerUuid = agentPeerId;
        ConfigRaw = configRaw ?? throw new ArgumentNullException(nameof(configRaw));
        Status = PeerStatus.Active;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public PeerId Id { get; }
    public Guid UserId { get; private set; }
    public Guid ServerId { get; private set; }
    public AgentPeerUuid AgentPeerUuid { get; private set; }
    public string ConfigRaw { get; private set; }
    public PeerStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; private set; }

    
    public static Peer CreateNew(Guid userId, Guid serverId, AgentPeerUuid agentPeerId, string configRaw, DateTime nowUtc, DateTime? expiresAt = null) =>
        new(PeerId.New(), userId, serverId, agentPeerId, configRaw, nowUtc, expiresAt);

    public void Revoke()
    {
        if (Status == PeerStatus.Revoked) return;
        Status = PeerStatus.Revoked;
    }

    
    public bool IsActive() =>
        Status == PeerStatus.Active && (ExpiresAt is null || ExpiresAt > DateTime.UtcNow);
    
    
    public void Renew(DateTime newExpiresAt)
    {
        if (Status != PeerStatus.Active)
            throw new InvalidOperationException("Cannot renew revoked peer");

        if (newExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration must be in the future", nameof(newExpiresAt));

        ExpiresAt = newExpiresAt;
    }

}