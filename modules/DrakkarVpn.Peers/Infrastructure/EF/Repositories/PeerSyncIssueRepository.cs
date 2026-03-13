using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerSyncIssueRepository : IPeerSyncIssueRepository
{
    private readonly PeerDbContext _db;

    public PeerSyncIssueRepository(PeerDbContext db)
    {
        _db = db;
    }

    public async Task InsertManyAsync(Guid serverId, IReadOnlyCollection<PeerSyncIssueItem> items, CancellationToken ct)
    {
        var entities = items.Select(i => new PeerSyncIssueEntity
        {
            Id            = Guid.NewGuid(),
            ServerId      = serverId,
            PeerId        = i.PeerId,
            AgentPeerUuid   = i.AgentPeerId,
            Type          = i.Type,
            DetectedAtUtc = i.DetectedAtUtc,
            Details       = i.Details
        });

        await _db.PeerSyncIssues.AddRangeAsync(entities, ct);
    }
}