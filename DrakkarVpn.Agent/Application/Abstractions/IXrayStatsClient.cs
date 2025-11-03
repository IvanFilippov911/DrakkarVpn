using Xray.App.Stats.Command;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IXrayStatsClient
{
    Task<IReadOnlyList<Stat>> GetUserStatsAsync(CancellationToken ct);
}