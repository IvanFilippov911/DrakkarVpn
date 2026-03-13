using System.Text.Json;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.PeerTraffic;

public sealed class PeerTrafficAlertFactory : IPeerTrafficAlertFactory
{
    private const long BytesPerGb = 1024L * 1024 * 1024;

    private const double WindowMinutes              = 10;
    private const double HardLimitGbPerWindow       = 5.0;
    private const double HighSpeedMbpsThreshold     = 300;
    private const long   OnlineMinBytesForSuspicion = 1L * BytesPerGb;

    public IReadOnlyList<CreateCoreAlertArgs> Build(SuspiciousPeerTrafficDto s)
    {
        var alerts = new List<CreateCoreAlertArgs>();

        if (!s.WasOnline || s.TotalBytes == 0)
            return alerts;

        var windowMinutes = (s.ToUtc - s.FromUtc).TotalMinutes;
        if (windowMinutes <= 0) windowMinutes = WindowMinutes;

        var gb        = s.TotalBytes / (double)BytesPerGb;
        var gbPerHour = gb * (60.0 / windowMinutes);

        if (gb >= HardLimitGbPerWindow)
        {
            alerts.Add(MakeAlert(
                code:     "USER_TRAFFIC_HUGE_VOLUME",
                severity: "Warning",
                title:    $"Подозрительный трафик пира {s.PeerId} на сервере {s.ServerId}",
                message:  $"За {windowMinutes:F0} мин. передано {gb:F2} GB.",
                s));
        }

        if (s.MaxSpeedMbps >= HighSpeedMbpsThreshold)
        {
            alerts.Add(MakeAlert(
                code:     "USER_TRAFFIC_HIGH_SPEED",
                severity: "Warning",
                title:    $"Высокая скорость трафика у пира {s.PeerId} на сервере {s.ServerId}",
                message:  $"Пиковая скорость ≈ {s.MaxSpeedMbps:F1} Mbps.",
                s));
        }
        
        if (s.TotalBytes >= OnlineMinBytesForSuspicion)
        {
            alerts.Add(MakeAlert(
                code:     "USER_TRAFFIC_LARGE_VOLUME_ONLINE",
                severity: "Info",
                title:    $"Крупный объём трафика у пира {s.PeerId} на сервере {s.ServerId}",
                message:  $"За окно передано {gb:F2} GB (≈ {gbPerHour:F2} GB/час).",
                s));
        }

        return alerts;
    }

    private static CreateCoreAlertArgs MakeAlert(
        string code,
        string severity,
        string title,
        string message,
        SuspiciousPeerTrafficDto s)
    {
        var details = new
        {
            s.PeerId,
            s.ServerId,
            s.FromUtc,
            s.ToUtc,
            s.TotalRxBytes,
            s.TotalTxBytes,
            s.TotalBytes,
            s.MaxSpeedMbps,
            s.WasOnline
        };

        return new CreateCoreAlertArgs(
            Source:      "UserTraffic",
            Code:        code,
            Severity:    severity,
            Title:       title,
            Message:     message,
            ServerId:    s.ServerId,
            UserId:      null,
            Region:      null,
            DetailsJson: JsonSerializer.Serialize(details)
        );
    }
}