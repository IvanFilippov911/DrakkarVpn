namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record CoreHealthDto(
    double Rps,
    double AvgLatencyMs,
    double ErrorRatePct,
    long   TotalRequests,
    long   TotalErrors,
    DateTime WindowStartUtc,
    DateTime WindowEndUtc,
    IReadOnlyDictionary<string, double> ErrorsByArea
)
{
    public static CoreHealthDto Empty()
    {
        var now = DateTime.UtcNow;

        return new CoreHealthDto(
            Rps: 0,
            AvgLatencyMs: 0,
            ErrorRatePct: 0,
            TotalRequests: 0,
            TotalErrors: 0,
            WindowStartUtc: now.AddSeconds(-5),
            WindowEndUtc: now,
            ErrorsByArea: new Dictionary<string, double>()
        );
    }
};