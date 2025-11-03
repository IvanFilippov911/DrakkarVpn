using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CleanupPeerMetricsHistory;

public sealed record CleanupPeerMetricsHistoryCommand(TimeSpan Ttl) : IRequest<Unit>;