namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record CoreHealthHistoryPointDto(
    DateTime TimestampUtc,
    double Rps,
    double AvgLatencyMs,
    double ErrorRatePct,
    long TotalErrors,
    IReadOnlyDictionary<string, long>? ErrorsByArea
);