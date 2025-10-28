using DrakkarVpn.Core.Api.Infrastructure.Absractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Peers.Domain;

public sealed class Peer : IAggregateRoot
{
    public PeerId Id { get; private set; }
    public Guid ServerId { get; private set; }
    public AgentPeerUuid AgentPeerUuid { get; private set; }
    public string ConfigRaw { get; private set; }
    public PeerStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string DeviceId { get; private set; }         
    public DateTime? LastHandshakeAt { get; private set; }
    
    private Peer() { }

    private Peer(
        PeerId id,
        Guid serverId,
        AgentPeerUuid agentPeerUuid,
        string configRaw,
        DateTime createdAt,
        string deviceId)
    {
        Id = id;
        ServerId = serverId;
        AgentPeerUuid = agentPeerUuid;
        ConfigRaw = configRaw ?? throw new ArgumentNullException(nameof(configRaw));
        Status = PeerStatus.Active;
        CreatedAt = createdAt;

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
    }

    public static Peer CreateNew(
        Guid serverId,
        AgentPeerUuid agentPeerUuid,
        string configRaw,
        string deviceId,
        DateTime nowUtc)
        => new(PeerId.New(), serverId, agentPeerUuid, configRaw, nowUtc,
               deviceId);

    public void Revoke() => Status = PeerStatus.Revoked;

    public bool IsActive() => Status == PeerStatus.Active;

    public void TouchHandshake(DateTime nowUtc) => LastHandshakeAt = nowUtc;
    
}