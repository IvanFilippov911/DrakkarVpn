namespace DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;

public sealed class AdminServerMetricsHistoryReadEntity
{
    public DateTime PeriodStartUtc { get; set; }
    public Guid ServerId { get; set; }

    public long TrafficRxDeltaBytes { get; set; }
    public long TrafficTxDeltaBytes { get; set; }

    public decimal VpnSpeedMbps { get; set; }
    public decimal InfraLatencyMs { get; set; }
}

