namespace DrakkarVpn.Shared.Servers;

public enum DeleteServerResult
{
    Deleted = 0,
    NotFound = 1,
    HasPeers = 2
}