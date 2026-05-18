using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Servers.Infrastructure.EF.Repositories.ServerTransportActivation;

public sealed class AgentApplyServerTransportContextRepository : IAgentApplyServerTransportContextRepository
{
    private readonly ServerDbContext _db;

    public AgentApplyServerTransportContextRepository(ServerDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyDictionary<Guid, AgentApplyServerTransportContext>> GetByActivationIdsAsync(
        IReadOnlyCollection<Guid> activationIds,
        CancellationToken ct)
    {
        if (activationIds.Count == 0)
            return new Dictionary<Guid, AgentApplyServerTransportContext>();

        var query =
            from a in _db.Set<ServerTransportProfileActivation>().AsNoTracking()
            join s in _db.Servers.AsNoTracking() on a.ServerId equals s.Id
            join p in _db.ServerTransportProfiles.AsNoTracking() on a.TransportProfileId equals p.Id
            where activationIds.Contains(a.Id)
            select new AgentApplyServerTransportContext(
                s.Id,
                a.Id,
                s.AgentBaseUrl.ToString(),
                s.PublicHost.Value,
                s.PublicPort,
                a.RealityPublicKey,
                p.TransportType,
                p.SecurityType,
                p.RealitySni,
                p.RealityShortId,
                p.RealityFingerprint,
                p.RealityDest,
                p.GrpcServiceName,
                p.GrpcAuthority);

        var rows = await query.ToListAsync(ct);

        var map = new Dictionary<Guid, AgentApplyServerTransportContext>(rows.Count);
        foreach (var row in rows)
            map[row.ActivationId] = row;

        return map;
    }
}
