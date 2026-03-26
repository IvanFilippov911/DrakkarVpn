
namespace DrakkarVpn.Core.Api.Modules.Peers.Domain;

public sealed class Peer
{
    public Guid Id { get; private set; }
    public Guid ServerId { get; private set; }
    public Guid AgentPeerUuid { get; private set; }
    public string DeviceId { get; private set; }
    public PeerStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime StatusUpdatedAtUtc { get; private set; } 

    public DateTime? LastDataAt { get; private set; }
    public long TotalRxBytes { get; private set; }
    public long TotalTxBytes { get; private set; }
    public double? SpeedMbps { get; private set; }
    public DateTime? LastPolledAt { get; private set; }
    public bool IsOnline { get; private set; }

    public double? VpnLatencyMs { get; private set; }
    public DateTime? LastLatencyAt { get; private set; }

    private static readonly TimeSpan OnlineThreshold = TimeSpan.FromSeconds(120);

    private Peer() { }

    private Peer(
        Guid id,
        Guid serverId,
        Guid agentPeerUuid,
        string deviceId,
        DateTime createdAtUtc)
    {
        Id          = id;
        ServerId    = serverId;
        AgentPeerUuid = agentPeerUuid;

        DeviceId = !string.IsNullOrWhiteSpace(deviceId)
            ? deviceId
            : throw new ArgumentNullException(nameof(deviceId));

        CreatedAt = EnsureUtc(createdAtUtc);

        Status = PeerStatus.Active;
        StatusUpdatedAtUtc = CreatedAt; 
    }

    public static Peer CreateNew(
        Guid serverId,
        Guid agentPeerUuid,
        string deviceId,
        DateTime nowUtc)
        => new(Guid.NewGuid(), serverId, agentPeerUuid, deviceId, nowUtc);

    public bool IsActive() => Status == PeerStatus.Active;
    
    public void RefreshOnlineStatus(DateTime nowUtc)
    {
        nowUtc = EnsureUtc(nowUtc);

        if (Status != PeerStatus.Active)
        {
            IsOnline = false;
            return;
        }

        IsOnline = LastDataAt.HasValue &&
                   nowUtc - LastDataAt.Value <= OnlineThreshold;
    }

    public void TouchData(DateTime atUtc)
    {
        atUtc = EnsureUtc(atUtc);
        if (!LastDataAt.HasValue || atUtc > LastDataAt.Value)
            LastDataAt = atUtc;
    }

    public void AddTraffic(long rxDelta, long txDelta, DateTime polledAtUtc)
    {
        polledAtUtc = EnsureUtc(polledAtUtc);

        if (Status != PeerStatus.Active)
            return;
        if (LastPolledAt.HasValue && polledAtUtc <= LastPolledAt.Value)
            return;

        if (LastPolledAt.HasValue &&
            polledAtUtc - LastPolledAt.Value > TimeSpan.FromHours(1))
        {
            TotalRxBytes = Math.Max(0, rxDelta);
            TotalTxBytes = Math.Max(0, txDelta);
            LastPolledAt = polledAtUtc;
            SpeedMbps    = null;

            if (rxDelta > 0 || txDelta > 0)
                TouchData(polledAtUtc);

            return;
        }

        if (rxDelta < 0) rxDelta = 0;
        if (txDelta < 0) txDelta = 0;

        const long maxDelta = 10L * 1024 * 1024 * 1024;
        if (rxDelta > maxDelta) rxDelta = maxDelta;
        if (txDelta > maxDelta) txDelta = maxDelta;

        if (LastPolledAt.HasValue)
        {
            var deltaBytes = rxDelta + txDelta;
            var dtSeconds  = (polledAtUtc - LastPolledAt.Value).TotalSeconds;

            if (dtSeconds > 0)
            {
                if (deltaBytes <= 0)
                {
                    SpeedMbps = 0;
                }
                else
                {
                    var bits  = deltaBytes * 8d;
                    var speed = bits / dtSeconds / 1_000_000d;
                    SpeedMbps = Math.Round(speed, 2);
                }
            }
        }
        else
        {
            SpeedMbps = null;
        }

        checked
        {
            TotalRxBytes += rxDelta;
            TotalTxBytes += txDelta;
        }

        LastPolledAt = polledAtUtc;

        if (rxDelta > 0 || txDelta > 0)
            TouchData(polledAtUtc);
    }

    public void UpdateVpnLatencyMs(double latencyMs, DateTime atUtc)
    {
        atUtc = EnsureUtc(atUtc);
        if (latencyMs < 0) return;
        VpnLatencyMs  = latencyMs;
        LastLatencyAt = atUtc;
    }

    private static DateTime EnsureUtc(DateTime t)
    {
        if (t.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Timestamp must be UTC", nameof(t));
        return t;
    }
}