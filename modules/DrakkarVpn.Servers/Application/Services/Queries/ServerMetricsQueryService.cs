using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

namespace DrakkarVpn.Servers.Application.Services.Queries;

public sealed class ServerMetricsQueryService : IServerMetricsQueryService
{
    private readonly IServerMetricsHistoryRepository _history;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ServerMetricsQueryService(
        IServerMetricsHistoryRepository history,
        IDateTimeProvider dateTimeProvider)
    {
        _history = history;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryAsync(
        Guid serverId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct)
    {
        var to = EnsureUtc(toUtc ?? _dateTimeProvider.UtcNow.UtcDateTime);
        var from = EnsureUtc(fromUtc ?? to.AddHours(-24));

        var rows = await _history.GetRangeAsync(serverId, from, to, ct);

        return rows.Select(x => new ServerMetricsHistoryDto(
            x.PeriodStartUtc,
            x.ServerId,
            x.Reachable,
            x.TrafficRxDeltaBytes,
            x.TrafficTxDeltaBytes,
            x.VpnSpeedMbps,
            x.InfraLatencyMs
        )).ToList();
    }

    public Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryLastAsync(
        Guid serverId,
        int minutes,
        CancellationToken ct)
    {
        minutes = minutes <= 0 ? 60 : minutes;
        var to = _dateTimeProvider.UtcNow.UtcDateTime;
        var from = to.AddMinutes(-minutes);
        return GetHistoryAsync(serverId, from, to, ct);
    }

    public Task<Dictionary<Guid, long>> GetTrafficSummaryAsync(
        Guid[] serverIds,
        DateTime fromUtc,
        CancellationToken ct)
    {
        if (serverIds is null || serverIds.Length == 0)
            return Task.FromResult(new Dictionary<Guid, long>());

        return _history.GetTrafficSumAsync(serverIds, EnsureUtc(fromUtc), ct);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
