namespace DrakkarVpn.Core.Api.Modules.Peers.Domain;

public enum PeerStatus { Active, Revoked, Expired }

public readonly record struct PeerId(Guid Value)
{
    public static PeerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct AgentPeerUuid(Guid Value)
{
    public override string ToString() => Value.ToString();
}