namespace DrakkarVpn.Core.Api.Diagnostics;

public sealed record CoreMetricsHistoryEntry(
    DateTime TimestampUtc,
    double Rps,
    double AvgLatencyMs,
    double ErrorRatePct,
    long TotalErrors,
    IReadOnlyDictionary<string, long>? ErrorsByArea
);