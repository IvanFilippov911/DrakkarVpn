namespace DrakkarVpn.Core.Api.Diagnostics;

public sealed record CoreRequestMetricsSnapshot(
    DateTime WindowStartUtc,
    DateTime WindowEndUtc,
    IReadOnlyDictionary<string, CoreCommandMetrics> PerCommand,
    IReadOnlyDictionary<string, long> ErrorsByArea
)
{
    public long TotalRequests  => PerCommand.Values.Sum(x => x.SuccessCount + x.ErrorCount);
    public long TotalErrors    => PerCommand.Values.Sum(x => x.ErrorCount);
    public long TotalElapsedMs => PerCommand.Values.Sum(x => x.TotalElapsedMs);

    public double? AvgLatencyMs =>
        TotalRequests > 0 ? (double)TotalElapsedMs / TotalRequests : null;

    public double ErrorRatePercent =>
        TotalRequests > 0 ? (double)TotalErrors / TotalRequests * 100.0 : 0;
}