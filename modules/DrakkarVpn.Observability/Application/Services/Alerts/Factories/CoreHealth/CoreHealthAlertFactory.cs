using System.Text.Json;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Services.Alerts.CoreHealth;
using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.CoreHealth;

public sealed class CoreHealthAlertFactory : ICoreHealthAlertFactory
{
    private const double ErrorRateCritical = 5.0;
    private const double LatencyWarning    = 300.0;
    private const double RpsWarning        = 500.0;

    private const double AreaDominanceThreshold = 0.7;
    private const long   AreaMinErrors          = 10;

    public IReadOnlyList<CreateCoreAlertArgs> Build(CoreHealthDto m)
    {
        var alerts = new List<CreateCoreAlertArgs>();

        if (m.TotalRequests == 0)
            return alerts;

        if (m.ErrorRatePct >= ErrorRateCritical)
        {
            alerts.Add(MakeAlert(
                code:     "CORE_ERROR_RATE_HIGH",
                severity: "Critical",
                title:    "Высокий процент ошибок в Core",
                message:  $"ErrorRate={m.ErrorRatePct:F2}% за окно.",
                details: new
                {
                    m.ErrorRatePct,
                    m.TotalRequests,
                    m.TotalErrors,
                    m.Rps,
                    m.AvgLatencyMs,
                    m.WindowStartUtc,
                    m.WindowEndUtc
                }));
        }

        if (m.AvgLatencyMs > LatencyWarning)
        {
            alerts.Add(MakeAlert(
                code:     "CORE_LATENCY_HIGH",
                severity: "Warning",
                title:    "Высокая средняя латентность Core",
                message:  $"AvgLatency={m.AvgLatencyMs:F1} ms.",
                details: new
                {
                    m.AvgLatencyMs,
                    m.Rps,
                    m.ErrorRatePct,
                    m.WindowStartUtc,
                    m.WindowEndUtc
                }));
        }

        if (m.Rps > RpsWarning)
        {
            alerts.Add(MakeAlert(
                code:     "CORE_RPS_HIGH",
                severity: "Warning",
                title:    "Высокий RPS в Core",
                message:  $"Rps={m.Rps:F1} req/s.",
                details: new
                {
                    m.Rps,
                    m.AvgLatencyMs,
                    m.ErrorRatePct,
                    m.TotalRequests,
                    m.WindowStartUtc,
                    m.WindowEndUtc
                }));
        }

        if (m.TotalErrors >= AreaMinErrors && m.ErrorsByArea.Count > 0)
        {
            var dominant = m.ErrorsByArea
                .OrderByDescending(x => x.Value)
                .First();

            var share = dominant.Value / (double)m.TotalErrors;

            if (share >= AreaDominanceThreshold)
            {
                alerts.Add(MakeAlert(
                    code:     "CORE_ERRORS_DOMINANT_AREA",
                    severity: "Warning",
                    title:    $"Большинство ошибок в зоне {dominant.Key}",
                    message:  $"Area={dominant.Key}, {dominant.Value} из {m.TotalErrors} ошибок ({share:P0}).",
                    details: new
                    {
                        Area        = dominant.Key,
                        AreaErrors  = dominant.Value,
                        TotalErrors = m.TotalErrors,
                        Share       = share,
                        m.ErrorRatePct,
                        m.Rps,
                        m.AvgLatencyMs,
                        m.WindowStartUtc,
                        m.WindowEndUtc
                    }));
            }
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
            Source:      "CoreHealth",
            Code:        code,
            Severity:    severity,
            Title:       title,
            Message:     message,
            ServerId:    null,
            UserId:      null,
            Region:      null,
            DetailsJson: detailsJson
        );
    }
}