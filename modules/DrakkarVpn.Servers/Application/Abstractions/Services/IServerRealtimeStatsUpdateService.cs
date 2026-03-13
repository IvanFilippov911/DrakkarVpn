namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerRealtimeStatsUpdateService
{
    Task<int> UpdateNowAsync(DateTime nowUtc, CancellationToken ct);
}