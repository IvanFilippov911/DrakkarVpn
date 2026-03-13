namespace DrakkarVpn.Agent.Application.Exception;

public sealed class PeersAgentAlreadyExistsException : System.Exception
{
    public PeersAgentAlreadyExistsException() : base("Peer already exists") { }
}