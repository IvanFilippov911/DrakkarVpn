using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Servers;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerPollResultApplyService : IServerPollResultApplyService
{
    private readonly IServerPollResultApplyRepository _repo;
    private readonly IServerAlertFactory _alerts;
    private readonly ICoreAlertService _coreAlertService;

    public ServerPollResultApplyService(
        IServerPollResultApplyRepository repo,
        ICoreAlertService coreAlertService,
        IServerAlertFactory alerts)
    {
        _repo = repo;
        _alerts = alerts;
        _coreAlertService = coreAlertService;
    }

    public async Task ApplyPollResultsAsync(
        IReadOnlyCollection<ServerPollResultDto> appliedResults,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (appliedResults is null) throw new ArgumentNullException(nameof(appliedResults));
        if (appliedResults.Count == 0) return;

        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        var updates = await _repo.ApplyBatchAsync(appliedResults, nowUtc, ct);
        if (updates.Count == 0) return;

        foreach (var u in updates)
        {
            var cmds = _alerts.Build(u, nowUtc);
            await _coreAlertService.CreateBatchAsync(cmds, ct);
        }
    }
}