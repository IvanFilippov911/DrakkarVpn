using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerRealtimeStatsUpsertService : IServerRealtimeStatsUpsertService
{
    private readonly IServerRealtimeStatsRepository _repo;

    public ServerRealtimeStatsUpsertService(IServerRealtimeStatsRepository repo)
        => _repo = repo;

    public Task UpsertManyAsync(
        IReadOnlyCollection<ServerRealtimeStatsUpsertRow> rows,
        DateTime nowUtc,
        CancellationToken ct)
        => _repo.UpsertManyAsync(rows, nowUtc, ct);
}