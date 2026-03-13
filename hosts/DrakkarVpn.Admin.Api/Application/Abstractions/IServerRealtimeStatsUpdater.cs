namespace DrakkarVpn.Admin.Api.Application.Abstractions;

public interface IServerRealtimeStatsUpdater
{
    Task<int> UpdateNowAsync(DateTime nowUtc, CancellationToken ct);
}