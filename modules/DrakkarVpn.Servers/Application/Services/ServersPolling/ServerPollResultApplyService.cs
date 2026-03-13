using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerPollResultApplyService : IServerPollResultApplyService
{
    private readonly IServerPollResultApplyRepository _repo;
    private readonly IMediator _mediator;
    private readonly IServerAlertFactory _alerts;

    public ServerPollResultApplyService(
        IServerPollResultApplyRepository repo,
        IMediator mediator,
        IServerAlertFactory alerts)
    {
        _repo = repo;
        _mediator = mediator;
        _alerts = alerts;
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
            foreach (var cmdAlert in cmds)
                await _mediator.Send(cmdAlert, ct);
        }
    }
}