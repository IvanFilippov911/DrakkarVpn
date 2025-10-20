using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.CleanupServerMetricsHistory;

public sealed class CleanupServerMetricsHistoryHandler
    : IRequestHandler<CleanupServerMetricsHistoryRequest, Unit>
{
    private readonly IServerMetricsHistoryRepository _repo;
    public CleanupServerMetricsHistoryHandler(IServerMetricsHistoryRepository repo) => _repo = repo;

    public async Task<Unit> Handle(CleanupServerMetricsHistoryRequest c, CancellationToken ct)
    {
        await _repo.DeleteOlderThanAsync(c.Ttl, ct);
        return Unit.Value;
    }
}