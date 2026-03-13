namespace DrakkarVpn.Core.Api.Diagnostics;

public sealed record CoreCommandMetrics(
    long SuccessCount,
    long ErrorCount,
    long TotalElapsedMs
);