using System.Diagnostics;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.ServersPolling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Handlers.RunServersPolling;

public sealed class RunServersPollingHandler
    : IRequestHandler<RunServersPollingCommand, Unit>
{
    private readonly IServerPollStateService _state;
    private readonly IAgentPollingService _agent;
    private readonly IServerPollResultApplyService _domainApply;
    private readonly IServerMetricsHistoryService _history;
    private readonly IServersPollingAlertFactory _cycleAlerts;
    private readonly ICoreAlertService _alert;
    private readonly ILogger<RunServersPollingHandler> _log;

    private readonly string _instanceId = Environment.MachineName;

    public RunServersPollingHandler(
        IServerPollStateService state,
        IAgentPollingService agent,
        IServerPollResultApplyService domainApply,
        IServerMetricsHistoryService history,
        IServersPollingAlertFactory cycleAlerts,
        ICoreAlertService alert,
        ILogger<RunServersPollingHandler> log)
    {
        _state = state;
        _agent = agent;
        _domainApply = domainApply;
        _history = history;
        _cycleAlerts = cycleAlerts;
        _alert = alert;
        _log = log;
    }

    public async Task<Unit> Handle(RunServersPollingCommand cmd, CancellationToken ct)
    {
        var startedAt = DateTime.UtcNow;
        var sw = Stopwatch.StartNew();

        var candidates = await _state.AcquireBatchAsync(
            nowUtc: startedAt,
            batchSize: cmd.BatchSize,
            leaseDuration: cmd.LeaseDuration,
            stuckTimeout: cmd.StuckTimeout,
            instanceId: _instanceId,
            ct: ct);

        if (candidates.Count == 0)
            return await FinishAsync(startedAt, sw, acquired: 0, applied: 0, ct);

        // 1) poll agents
        var results = await _agent.PollBatchAsync(
            candidates,
            cmd.HttpConcurrency,
            ct);

        // 2) apply state
        var applyUtc = DateTime.UtcNow;
        var stateApply = await _state.ApplyResultsAsync(
            results,
            applyUtc,
            _instanceId,
            ct);

        var appliedIds = stateApply.AppliedServerIds;
        if (appliedIds.Count == 0)
            return await FinishAsync(startedAt, sw, acquired: candidates.Count, applied: 0, ct);

        // 3) domain update
        var appliedSet = appliedIds.ToHashSet();
        var appliedResults = results
            .Where(r => appliedSet.Contains(r.ServerId))
            .ToList();

        if (appliedResults.Count > 0)
            await _domainApply.ApplyPollResultsAsync(appliedResults, DateTime.UtcNow, ct);

        // 4) metrics history from state
        var periodStartUtc = TruncateTo10sUtc(DateTime.UtcNow);
        await _history.AppendFromStateAsync(
            appliedIds,
            periodStartUtc,
            DateTime.UtcNow,
            ct);

        return await FinishAsync(
            startedAt,
            sw,
            acquired: candidates.Count,
            applied: appliedIds.Count,
            ct);
    }

    private async Task<Unit> FinishAsync(
        DateTime startedAt,
        Stopwatch sw,
        int acquired,
        int applied,
        CancellationToken ct)
    {
        sw.Stop();

        var finishedAt = DateTime.UtcNow;
        var durationMs = sw.Elapsed.TotalMilliseconds;

        await PublishCycleAlertsAsync(
            startedAt,
            finishedAt,
            acquired,
            applied,
            durationMs,
            ct);

        _log.LogInformation(
            "Servers polling cycle finished: acquired={Acquired}, applied={Applied}, durationMs={DurationMs}",
            acquired,
            applied,
            durationMs);

        return Unit.Value;
    }

    private async Task PublishCycleAlertsAsync(
        DateTime startedAt,
        DateTime finishedAt,
        int acquired,
        int applied,
        double durationMs,
        CancellationToken ct)
    {
        var cycle = new ServersPollingCycleDto(
            StartedAtUtc: startedAt,
            FinishedAtUtc: finishedAt,
            AcquiredCount: acquired,
            AppliedCount: applied,
            DurationMs: durationMs);

        var alerts = _cycleAlerts.Build(cycle);
        if (alerts.Count == 0)
            return;

        await _alert.CreateBatchAsync(alerts, ct);
    }

    private static DateTime TruncateTo10sUtc(DateTime utc)
    {
        utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        var sec = utc.Second - (utc.Second % 10);
        return new DateTime(
            utc.Year,
            utc.Month,
            utc.Day,
            utc.Hour,
            utc.Minute,
            sec,
            DateTimeKind.Utc);
    }
}