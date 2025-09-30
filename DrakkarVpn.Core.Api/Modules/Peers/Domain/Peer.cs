using DrakkarVpn.Core.Api.Infrastructure.Absractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Peers.Domain;

public sealed class Peer : IAggregateRoot
{
    public PeerId Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ServerId { get; private set; }
    public SubscriptionId SubscriptionId { get; private set; }   // ✅ ValueObject вместо Guid
    public AgentPeerUuid AgentPeerUuid { get; private set; }
    public string ConfigRaw { get; private set; }
    public PeerStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private Peer(
        PeerId id,
        Guid userId,
        Guid serverId,
        SubscriptionId subscriptionId,
        AgentPeerUuid agentPeerUuid,
        string configRaw,
        DateTime createdAt,
        DateTime? expiresAt)
    {
        Id = id;
        UserId = userId;
        ServerId = serverId;
        SubscriptionId = subscriptionId;
        AgentPeerUuid = agentPeerUuid;
        ConfigRaw = configRaw ?? throw new ArgumentNullException(nameof(configRaw));
        Status = PeerStatus.Active;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static Peer CreateNew(
        Guid userId,
        Guid serverId,
        SubscriptionId subscriptionId,
        AgentPeerUuid agentPeerUuid,
        string configRaw,
        DateTime nowUtc,
        DateTime? expiresAt = null)
        => new(PeerId.New(), userId, serverId, subscriptionId, agentPeerUuid, configRaw, nowUtc, expiresAt);

    public void Revoke() => Status = PeerStatus.Revoked;

    public bool IsActive() => Status == PeerStatus.Active && (ExpiresAt is null || ExpiresAt > DateTime.UtcNow);
    
    
    public void Renew(DateTime newExpiresAt)
    {
        if (Status != PeerStatus.Active)
            throw new InvalidOperationException("Cannot renew revoked peer");

        if (newExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration must be in the future", nameof(newExpiresAt));

        ExpiresAt = newExpiresAt;
    }

}