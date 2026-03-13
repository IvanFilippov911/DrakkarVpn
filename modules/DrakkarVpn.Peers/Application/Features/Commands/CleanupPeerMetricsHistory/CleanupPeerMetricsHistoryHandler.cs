using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CleanupPeerMetricsHistory;

public sealed class CleanupPeerMetricsHistoryHandler
    : IRequestHandler<CleanupPeerMetricsHistoryCommand, Unit>
{
    private readonly IPeerMetricsHistoryRepository _repo;

    public CleanupPeerMetricsHistoryHandler(IPeerMetricsHistoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(CleanupPeerMetricsHistoryCommand cmd, CancellationToken ct)
    {
        await _repo.DeleteOlderThanAsync(cmd.Ttl, ct);
        return Unit.Value;
    }
}