namespace DrakkarVpn.Observability.Application.DTOs;

public sealed record ServersPollingCycleDto(
    DateTime StartedAtUtc,
    DateTime FinishedAtUtc,
    int AcquiredCount,
    int AppliedCount,
    double DurationMs
);