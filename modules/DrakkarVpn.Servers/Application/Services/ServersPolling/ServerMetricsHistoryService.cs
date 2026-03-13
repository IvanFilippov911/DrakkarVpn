using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerMetricsHistoryService : IServerMetricsHistoryService
{
    private readonly IServerMetricsHistoryRepository _repo;

    public ServerMetricsHistoryService(IServerMetricsHistoryRepository repo)
    {
        _repo = repo;
    }

    public Task AppendFromStateAsync(
        IReadOnlyCollection<Guid> appliedServerIds,
        DateTime periodStartUtc,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (appliedServerIds is null)
            throw new ArgumentNullException(nameof(appliedServerIds));

        if (appliedServerIds.Count == 0)
            return Task.CompletedTask;

        periodStartUtc = DateTime.SpecifyKind(periodStartUtc, DateTimeKind.Utc);
        nowUtc         = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        return _repo.AppendFromStateAsync(appliedServerIds, periodStartUtc, nowUtc, ct);
    }
}