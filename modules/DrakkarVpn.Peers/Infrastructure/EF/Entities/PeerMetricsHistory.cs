namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

public sealed class PeerMetricsHistory
{
    public Guid PeerId { get; set; }
    public Guid ServerId { get; set; }
    public DateTime PeriodStartUtc { get; set; }

    public long TotalRxBytes { get; set; }
    public long TotalTxBytes { get; set; }
    public long RxDeltaBytes { get; set; }
    public long TxDeltaBytes { get; set; }

    public bool IsOnline { get; set; }
    public double? SpeedMbps { get; private set; }

    public double? VpnLatencyMs { get; set; }
    
}