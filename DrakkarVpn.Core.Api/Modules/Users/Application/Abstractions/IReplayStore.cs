namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IReplayStore
{
    Task<bool> TryReserveAsync(string key, TimeSpan ttl, CancellationToken ct);
}