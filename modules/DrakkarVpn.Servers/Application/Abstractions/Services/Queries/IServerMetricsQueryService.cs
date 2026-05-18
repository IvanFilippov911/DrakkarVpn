using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

public interface IServerMetricsQueryService
{
    Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryAsync(
        Guid serverId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryLastAsync(
        Guid serverId,
        int minutes,
        CancellationToken ct);

    Task<Dictionary<Guid, long>> GetTrafficSummaryAsync(
        Guid[] serverIds,
        DateTime fromUtc,
        CancellationToken ct);
}
