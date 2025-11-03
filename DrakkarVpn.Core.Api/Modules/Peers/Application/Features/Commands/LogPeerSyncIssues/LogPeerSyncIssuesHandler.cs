using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands;

public sealed class LogPeerSyncIssuesHandler
    : IRequestHandler<LogPeerSyncIssuesCommand, Unit>
{
    private readonly IPeerSyncIssueRepository _repo;

    public LogPeerSyncIssuesHandler(IPeerSyncIssueRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(LogPeerSyncIssuesCommand cmd, CancellationToken ct)
    {
        if (cmd.Items.Count == 0)
            return Unit.Value;

        await _repo.InsertManyAsync(cmd.ServerId, cmd.Items, ct);
        return Unit.Value;
    }
}