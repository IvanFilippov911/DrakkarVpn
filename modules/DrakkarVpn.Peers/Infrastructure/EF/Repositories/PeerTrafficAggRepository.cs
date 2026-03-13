using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerTrafficAggRepository : IPeerTrafficAggRepository
{
    private readonly PeerDbContext _db;

    public PeerTrafficAggRepository(PeerDbContext db)
    {
        _db = db;
    }

    public async Task UpsertBatchAsync(
        IReadOnlyCollection<PeerTrafficAgg> items,
        CancellationToken ct)
    {
        if (items.Count == 0)
            return;

        var ids = items.Select(x => x.PeerId).ToArray();

        var existing = await _db.PeerTrafficAggs
            .Where(x => ids.Contains(x.PeerId))
            .ToDictionaryAsync(x => x.PeerId, ct);

        foreach (var item in items)
        {
            if (existing.TryGetValue(item.PeerId, out var row))
            {
                row.Last1hBytes  = item.Last1hBytes;
                row.Last24hBytes = item.Last24hBytes;
                row.UpdatedAtUtc = item.UpdatedAtUtc;
            }
            else
            {
                _db.PeerTrafficAggs.Add(item);
            }
        }
    }
}