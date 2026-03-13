namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Errors;

public sealed class PeersAgentAlreadyExistsException : Exception
{
    public Guid ServerId { get; }
    public Guid PeerUuid { get; }

    public PeersAgentAlreadyExistsException(Guid serverId, Guid peerUuid, string? message = null, Exception? inner = null)
        : base(message ?? "Peer already exists on agent", inner)
    {
        ServerId = serverId;
        PeerUuid = peerUuid;
    }
}