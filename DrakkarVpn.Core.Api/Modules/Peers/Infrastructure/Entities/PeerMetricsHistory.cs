namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

public sealed class PeerMetricsHistory
{
    public Guid PeerId { get; set; }
    public Guid ServerId { get; set; }
    public DateTime PeriodStartUtc { get; set; }

    public long TotalRxBytes { get; set; }
    public long TotalTxBytes { get; set; }

    public bool IsOnline { get; set; }

    public double? VpnLatencyMs { get; set; }
    
    public DateTime? LastDataAt { get; set; }
    public DateTime? LastLatencyAt { get; set; }
}