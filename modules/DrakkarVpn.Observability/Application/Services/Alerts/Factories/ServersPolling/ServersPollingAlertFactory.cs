using System.Text.Json;
using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.DTOs;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.ServersPolling;

public sealed class ServersPollingAlertFactory : IServersPollingAlertFactory
{
    private const double SlowWarnMs     = 5_000;
    private const double SlowCriticalMs = 15_000;
    
    private const double LowApplyRateWarn = 0.50;

    public IReadOnlyList<CreateCoreAlertArgs> Build(ServersPollingCycleDto s)
    {
        var alerts = new List<CreateCoreAlertArgs>();
        
        if (s.AcquiredCount == 0)
        {
            alerts.Add(MakeAlert(
                code: "SERVERS_POLLING_ZERO_ACQUIRED",
                severity: "Warning",
                title: "Servers polling: нет серверов для опроса",
                message: $"Цикл polling прошёл без захваченных серверов. Duration={s.DurationMs:F0} ms.",
                details: new
                {
                    s.StartedAtUtc,
                    s.FinishedAtUtc,
                    s.AcquiredCount,
                    s.AppliedCount,
                    s.DurationMs
                }));
            return alerts;
        }
        
        if (s.AppliedCount == 0)
        {
            alerts.Add(MakeAlert(
                code: "SERVERS_POLLING_ZERO_APPLIED",
                severity: "Critical",
                title: "Servers polling: результаты не применились",
                message: $"Захватили {s.AcquiredCount} серверов, но применилось 0. Duration={s.DurationMs:F0} ms.",
                details: new
                {
                    s.StartedAtUtc,
                    s.FinishedAtUtc,
                    s.AcquiredCount,
                    s.AppliedCount,
                    s.DurationMs
                }));
        }
        else
        {
            var rate = (double)s.AppliedCount / s.AcquiredCount;
            if (rate < LowApplyRateWarn)
            {
                alerts.Add(MakeAlert(
                    code: "SERVERS_POLLING_LOW_APPLY_RATE",
                    severity: "Warning",
                    title: "Servers polling: низкий apply-rate",
                    message: $"Applied {s.AppliedCount}/{s.AcquiredCount} ({rate:P0}). Duration={s.DurationMs:F0} ms.",
                    details: new
                    {
                        s.StartedAtUtc,
                        s.FinishedAtUtc,
                        s.AcquiredCount,
                        s.AppliedCount,
                        ApplyRate = rate,
                        s.DurationMs
                    }));
            }
        }
        
        if (s.DurationMs >= SlowCriticalMs)
        {
            alerts.Add(MakeAlert(
                code: "SERVERS_POLLING_SLOW_CYCLE",
                severity: "Critical",
                title: "Servers polling: критично долгий цикл",
                message: $"Цикл занял {s.DurationMs:F0} ms. acquired={s.AcquiredCount}, applied={s.AppliedCount}.",
                details: new
                {
                    s.StartedAtUtc,
                    s.FinishedAtUtc,
                    s.AcquiredCount,
                    s.AppliedCount,
                    s.DurationMs
                }));
        }
        else if (s.DurationMs >= SlowWarnMs)
        {
            alerts.Add(MakeAlert(
                code: "SERVERS_POLLING_SLOW_CYCLE",
                severity: "Warning",
                title: "Servers polling: долгий цикл",
                message: $"Цикл занял {s.DurationMs:F0} ms. acquired={s.AcquiredCount}, applied={s.AppliedCount}.",
                details: new
                {
                    s.StartedAtUtc,
                    s.FinishedAtUtc,
                    s.AcquiredCount,
                    s.AppliedCount,
                    s.DurationMs
                }));
        }

        return alerts;
    }

    private static CreateCoreAlertArgs MakeAlert(
        string code,
        string severity,
        string title,
        string message,
        object? details)
    {
        var detailsJson = details is null ? null : JsonSerializer.Serialize(details);

        return new CreateCoreAlertArgs(
            Source: "ServersPolling",
            Code: code,
            Severity: severity,
            Title: title,
            Message: message,
            ServerId: null,
            UserId: null,
            Region: null,
            DetailsJson: detailsJson
        );
    }
}