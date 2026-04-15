using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Servers;
using DrakkarVpn.Servers.Application.DTOs.ServerState;
using DrakkarVpn.Servers.Domain.Inputs;
using DrakkarVpn.Servers.Domain.Policies;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerPollResultApplyService : IServerPollResultApplyService
{
    private readonly IServerPollResultApplyRepository _repo;
    private readonly IServerAlertFactory _alerts;
    private readonly ICoreAlertService _coreAlertService;
    private readonly ILogger<ServerPollResultApplyService> _log;

    public ServerPollResultApplyService(
        IServerPollResultApplyRepository repo,
        ICoreAlertService coreAlertService,
        IServerAlertFactory alerts,
        ILogger<ServerPollResultApplyService> log)
    {
        _repo = repo;
        _alerts = alerts;
        _coreAlertService = coreAlertService;
        _log = log;
    }

    public async Task ApplyPollResultsAsync(
        IReadOnlyCollection<ServerPollResultDto> appliedResults,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (appliedResults is null) throw new ArgumentNullException(nameof(appliedResults));
        if (appliedResults.Count == 0) return;

        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        var serverIds = appliedResults
            .Select(x => x.ServerId)
            .Distinct()
            .ToArray();

        var stateByServerId = await _repo.GetStateAsync(serverIds, ct);

        var missingState = serverIds.Where(id => !stateByServerId.ContainsKey(id)).ToArray();
        if (missingState.Length > 0)
        {
            var sample = string.Join(", ", missingState.Take(20));
            if (missingState.Length > 20)
                sample += ", …";

            _log.LogWarning(
                "Server poll apply skipped {Count} server(s): no row in servers/server_poll_states (sample: {Sample})",
                missingState.Length,
                sample);
        }

        var updates = BuildUpdates(appliedResults, stateByServerId, nowUtc);
        if (updates.Count == 0) return;

        var infoUpdates = await _repo.ApplyUpdatesAsync(updates, ct);
        if (infoUpdates.Count == 0) return;

        var alertArgs = new List<CreateCoreAlertArgs>();
        foreach (var u in infoUpdates)
        {
            var cmds = _alerts.Build(u, nowUtc);
            if (cmds.Count > 0)
                alertArgs.AddRange(cmds);
        }

        if (alertArgs.Count > 0)
            await _coreAlertService.CreateBatchAsync(alertArgs, ct);
    }

    private static List<ServerUpdate> BuildUpdates(
        IReadOnlyCollection<ServerPollResultDto> appliedResults,
        IReadOnlyDictionary<Guid, ServerState> stateByServerId,
        DateTime nowUtc)
    {
        var updates = new List<ServerUpdate>(appliedResults.Count);

        foreach (var result in appliedResults)
        {
            if (!stateByServerId.TryGetValue(result.ServerId, out var state))
                continue;

            var input = new ServerPollResult(
                Reachable: result.Reachable,
                PeersActive: result.PeersActive,
                RxTotal: result.RxTotal,
                TxTotal: result.TxTotal,
                InfraLatencyMs: result.InfraLatencyMs,
                VpnSpeedMbps: result.VpnSpeedMbps,
                ObservedAtUtc: nowUtc,
                ConsecutiveFailures: state.ConsecutiveFailures
            );
            
            var newStatus = ServerStatusPolicy.Compute(
                state.Status,
                input,
                state.MaxPeers);

            updates.Add(new ServerUpdate(
                ServerId: result.ServerId,
                Status: newStatus,
                Reachable: input.Reachable,
                PeersActive: input.PeersActive,
                RxTotal: input.RxTotal,
                TxTotal: input.TxTotal,
                InfraLatencyMs: input.InfraLatencyMs,
                VpnSpeedMbps: input.VpnSpeedMbps,
                UpdatedAtUtc: input.ObservedAtUtc
            ));
        }

        return updates;
    }
}