namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

public sealed record BenchmarkSnapshot
{
    private BenchmarkSnapshot() { }

    public BenchmarkSnapshot(double maxSpeedMbps, DateTime measuredAt)
    {
        if (maxSpeedMbps < 0)
            throw new ArgumentOutOfRangeException(nameof(maxSpeedMbps), "Max speed cannot be negative.");

        MaxSpeedMbps = maxSpeedMbps;
        MeasuredAt = measuredAt;
    }

    public double MaxSpeedMbps { get; }
    public DateTime MeasuredAt { get; }

    public static BenchmarkSnapshot Empty => new(0, DateTime.MinValue);
}