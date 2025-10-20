using DrakkarVpn.Core.Api.Infrastructure.Absractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Peers.Domain;

public sealed class Peer : IAggregateRoot
{
    public PeerId Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ServerId { get; private set; }
    public SubscriptionId SubscriptionId { get; private set; }
    public AgentPeerUuid AgentPeerUuid { get; private set; }
    public string ConfigRaw { get; private set; }
    public PeerStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string DeviceId { get; private set; }         
    public DateTime? LastHandshakeAt { get; private set; }
    
    private Peer() { }

    private Peer(
        PeerId id,
        Guid userId,
        Guid serverId,
        SubscriptionId subscriptionId,
        AgentPeerUuid agentPeerUuid,
        string configRaw,
        DateTime createdAt,
        string deviceId,
        string? deviceName,
        string? platform)
    {
        Id = id;
        UserId = userId;
        ServerId = serverId;
        SubscriptionId = subscriptionId;
        AgentPeerUuid = agentPeerUuid;
        ConfigRaw = configRaw ?? throw new ArgumentNullException(nameof(configRaw));
        Status = PeerStatus.Active;
        CreatedAt = createdAt;

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
    }

    public static Peer CreateNew(
        Guid userId,
        Guid serverId,
        SubscriptionId subscriptionId,
        AgentPeerUuid agentPeerUuid,
        string configRaw,
        string deviceId,
        string? deviceName,
        string? platform,
        DateTime nowUtc)
        => new(PeerId.New(), userId, serverId, subscriptionId, agentPeerUuid, configRaw, nowUtc,
               deviceId, deviceName, platform);

    public void Revoke() => Status = PeerStatus.Revoked;

    public bool IsActive() => Status == PeerStatus.Active;

    public void TouchHandshake(DateTime nowUtc) => LastHandshakeAt = nowUtc;
    
}