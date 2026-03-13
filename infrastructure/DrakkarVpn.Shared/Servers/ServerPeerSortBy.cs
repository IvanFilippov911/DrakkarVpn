namespace DrakkarVpn.Shared.Servers;

public enum ServerPeerSortBy
{
    LastActivity = 0, 
    Traffic1h    = 1,
    Traffic24h   = 2, 
    Speed        = 3, 
    Latency      = 4, 
    CreatedAt    = 5  
}