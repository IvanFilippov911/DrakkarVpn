using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerRealtimeStatsRepository
{
    Task UpsertManyAsync(
        IReadOnlyCollection<ServerRealtimeStatsUpsertRow> rows,
        DateTime nowUtc,
        CancellationToken ct);
}