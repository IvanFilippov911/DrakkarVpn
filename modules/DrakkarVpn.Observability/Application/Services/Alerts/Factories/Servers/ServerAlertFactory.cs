using System.Text.Json;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.DTOs;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Servers;

public sealed class ServerAlertFactory : IServerAlertFactory
{
    private const double MaxBandwidthMbpsWarning = 900;
    private const double MaxInfraLatencyWarning  = 500;
    private const double PeersUtilizationWarn    = 0.95;

    public IReadOnlyList<CreateCoreAlertArgs> Build(ServerInfoUpdateDto u, DateTime nowUtc)
    {
        var alerts = new List<CreateCoreAlertArgs>();

        if (u.DisabledByFail)
            alerts.Add(BuildUnreachableAlert(u));

        if (u.MaxPeers is > 0 && u.PeersActive >= u.MaxPeers.Value * PeersUtilizationWarn)
            alerts.Add(BuildPeersAlmostFullAlert(u));

        if (u.VpnSpeedMbps > MaxBandwidthMbpsWarning)
            alerts.Add(BuildBandwidthAlert(u));

        if (u.InfraLatencyMs > MaxInfraLatencyWarning)
            alerts.Add(BuildLatencyAlert(u));

        if (u.NewStatus != u.OldStatus)
            alerts.Add(BuildStatusChangedAlert(u));

        return alerts;
    }

    private static CreateCoreAlertArgs BuildUnreachableAlert(ServerInfoUpdateDto u)
        => MakeAlert(
            source:   "Server",
            code:     "SERVER_UNREACHABLE_DISABLED",
            severity: "Critical",
            title:    $"Сервер {u.ServerId} отключен: 3 неудачных пинга",
            message:  $"Сервер {u.ServerId} был {u.ConsecutiveFailures} раз подряд недоступен и переведён в Disabled.",
            serverId: u.ServerId,
            details:  new { u.ConsecutiveFailures });

    private static CreateCoreAlertArgs BuildPeersAlmostFullAlert(ServerInfoUpdateDto u)
        => MakeAlert(
            source:   "Server",
            code:     "SERVER_PEERS_ALMOST_FULL",
            severity: "Warning",
            title:    $"Сервер {u.ServerId}: {u.PeersActive}/{u.MaxPeers} peers",
            message:  $"Peers заняты: {u.PeersActive} из {u.MaxPeers}.",
            serverId: u.ServerId,
            details:  new { u.PeersActive, u.MaxPeers });

    private static CreateCoreAlertArgs BuildBandwidthAlert(ServerInfoUpdateDto u)
        => MakeAlert(
            source:   "Server",
            code:     "SERVER_BANDWIDTH_HIGH",
            severity: "Warning",
            title:    $"Сервер {u.ServerId}: высокая скорость VPN",
            message:  $"Средняя скорость ≈ {u.VpnSpeedMbps:F1} Mbps.",
            serverId: u.ServerId,
            details:  new { u.VpnSpeedMbps });

    private static CreateCoreAlertArgs BuildLatencyAlert(ServerInfoUpdateDto u)
        => MakeAlert(
            source:   "Server",
            code:     "SERVER_LATENCY_HIGH",
            severity: "Warning",
            title:    $"Сервер {u.ServerId}: высокая infra latency",
            message:  $"Infra latency ≈ {u.InfraLatencyMs:F1} ms.",
            serverId: u.ServerId,
            details:  new { u.InfraLatencyMs });

    private static CreateCoreAlertArgs BuildStatusChangedAlert(ServerInfoUpdateDto u)
    {
        var severity = u.NewStatus switch
        {
            ServerStatus.Disabled => "Critical",
            ServerStatus.Draining => "Warning",
            _                     => "Info"
        };

        return MakeAlert(
            source:   "Server",
            code:     "SERVER_STATUS_CHANGED",
            severity: severity,
            title:    $"Сервер {u.ServerId}: статус {u.OldStatus} → {u.NewStatus}",
            message:  $"Статус сервера изменился с {u.OldStatus} на {u.NewStatus}. " +
                      $"Reachable={u.Reachable}, Peers={u.PeersActive}.",
            serverId: u.ServerId,
            details: new
            {
                OldStatus = u.OldStatus.ToString(),
                NewStatus = u.NewStatus.ToString(),
                u.Reachable,
                u.PeersActive,
                u.VpnSpeedMbps,
                u.InfraLatencyMs
            });
    }

    private static CreateCoreAlertArgs MakeAlert(
        string source,
        string code,
        string severity,
        string title,
        string message,
        Guid serverId,
        object? details)
    {
        var detailsJson = details is null ? null : JsonSerializer.Serialize(details);

        return new CreateCoreAlertArgs(
            Source:      source,
            Code:        code,
            Severity:    severity,
            Title:       title,
            Message:     message,
            ServerId:    serverId,
            UserId:      null,
            Region:      null,
            DetailsJson: detailsJson
        );
    }
}