namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;

public sealed class ServerRealtimeStats
{
    public Guid ServerId { get; private set; }

    public int OnlinePeers { get; private set; }
    public long TrafficLast1hBytes { get; private set; }
    public long TrafficLast24hBytes { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? TrafficCalculatedAtUtc { get; private set; }

    private ServerRealtimeStats() { }

    public ServerRealtimeStats(Guid serverId)
    {
        ServerId = serverId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Apply(
        int onlinePeers,
        long traffic1h,
        long traffic24h,
        DateTime nowUtc)
    {
        OnlinePeers         = onlinePeers;
        TrafficLast1hBytes  = traffic1h;
        TrafficLast24hBytes = traffic24h;
        UpdatedAtUtc        = nowUtc;
        TrafficCalculatedAtUtc = nowUtc;
    }
}