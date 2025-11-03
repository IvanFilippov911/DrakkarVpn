using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.UpsertPeerMetricsHistory;
using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.UpsertPeerMetrics;

public sealed class UpsertPeerMetricsHandler
    : IRequestHandler<UpsertPeerMetricsCommand, Unit>
{
    private readonly IPeerRepository _peers;
    private readonly IPeerMetricsProcessor _processor;
    private readonly IMediator _mediator;

    public UpsertPeerMetricsHandler(
        IPeerRepository peers,
        IPeerMetricsProcessor processor,
        IMediator mediator)
    {
        _peers = peers;
        _processor = processor;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(UpsertPeerMetricsCommand cmd, CancellationToken ct)
    {
        var nowUtc  = DateTime.UtcNow;
        var slot10s = TruncateTo10sUtc(nowUtc);

        var peersOnServer = await _peers.GetByServerAsync(cmd.ServerId, ct);

        var (history, syncIssues) = _processor.ApplyMetricsAndDetectIssues(
            cmd.ServerId,
            nowUtc,
            peersOnServer,
            cmd.Items);

        if (history.Count > 0)
        {
            await _mediator.Send(new UpsertPeerMetricsHistoryCommand(
                ServerId:       cmd.ServerId,
                PeriodStartUtc: slot10s,
                Items:          history
            ), ct);
        }

        if (syncIssues.Count > 0)
        {
            await _mediator.Send(new LogPeerSyncIssuesCommand(
                ServerId: cmd.ServerId,
                Items:    syncIssues
            ), ct);
        }

        return Unit.Value;
    }

    private static DateTime TruncateTo10sUtc(DateTime utc)
    {
        var sec = utc.Second - (utc.Second % 10);
        return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, sec, DateTimeKind.Utc);
    }
}