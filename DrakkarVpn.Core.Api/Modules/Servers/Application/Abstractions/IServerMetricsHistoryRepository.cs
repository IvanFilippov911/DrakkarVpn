using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerMetricsHistoryRepository
{
    Task AddOrUpdateMinuteAsync(ServerMetricsHistory e, CancellationToken ct);
    Task DeleteOlderThanAsync(TimeSpan ttl, CancellationToken ct);
    Task<List<ServerMetricsHistory>> GetRangeAsync(Guid serverId, DateTime fromUtc, DateTime toUtc, CancellationToken ct);
    Task<Dictionary<Guid, long>> GetTrafficSumAsync(
        Guid[] serverIds,
        DateTime fromUtc,
        CancellationToken ct);
}