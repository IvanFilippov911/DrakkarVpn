using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.UpsertPeerMetricsHistory;

public sealed record UpsertPeerMetricsHistoryCommand(
    Guid ServerId,
    DateTime PeriodStartUtc,
    IReadOnlyCollection<PeerMetricsHistoryItem> Items
) : IRequest<Unit>;