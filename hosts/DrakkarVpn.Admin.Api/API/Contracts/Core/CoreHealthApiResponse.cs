namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Core;

/// <summary>
/// API response contract for GET /api/admin/core/health.
/// </summary>
public sealed record CoreHealthApiResponse(
    double Rps,
    double AvgLatencyMs,
    double ErrorRatePct,
    long TotalRequests,
    long TotalErrors,
    DateTime WindowStartUtc,
    DateTime WindowEndUtc,
    IReadOnlyDictionary<string, double> ErrorsByArea);
