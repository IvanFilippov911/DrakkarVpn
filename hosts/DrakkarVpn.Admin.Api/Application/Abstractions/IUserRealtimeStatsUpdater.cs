namespace DrakkarVpn.Admin.Api.Application.Abstractions;

public interface IUserRealtimeStatsUpdater
{
    Task UpdateAllUsersAsync(CancellationToken ct);
}
