using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerTransportActivationReadRepository : IServerTransportActivationReadRepository
{
    private readonly ServerDbContext _db;

    public ServerTransportActivationReadRepository(ServerDbContext db)
        => _db = db;

    public Task<bool> ServerExistsAsync(Guid serverId, CancellationToken ct)
        => _db.Servers.AnyAsync(x => x.Id == serverId, ct);

    public async Task<IReadOnlyList<TransportProfileAttachCandidateDto>> GetTransportProfilesForAttachAsync(
        IReadOnlyCollection<Guid> transportProfileIds,
        CancellationToken ct)
    {
        if (transportProfileIds.Count == 0)
            return Array.Empty<TransportProfileAttachCandidateDto>();

        return await _db.ServerTransportProfiles
            .AsNoTracking()
            .Where(x => transportProfileIds.Contains(x.Id))
            .Select(x => new TransportProfileAttachCandidateDto(x.Id, x.IsEnabled))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlySet<Guid>> GetAttachedProfileIdsAsync(
        Guid serverId,
        IReadOnlyCollection<Guid> transportProfileIds,
        CancellationToken ct)
    {
        if (transportProfileIds.Count == 0)
            return new HashSet<Guid>();

        var ids = await _db.Set<ServerTransportProfileActivation>()
            .AsNoTracking()
            .Where(x => x.ServerId == serverId && transportProfileIds.Contains(x.TransportProfileId))
            .Select(x => x.TransportProfileId)
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    public async Task<IReadOnlyList<ServerTransportActivationListItemDto>> GetByServerAsync(Guid serverId, CancellationToken ct)
    {
        var items = await _db.Set<ServerTransportProfileActivation>()
            .AsNoTracking()
            .Where(x => x.ServerId == serverId)
            .Join(
                _db.ServerTransportProfiles.AsNoTracking(),
                activation => activation.TransportProfileId,
                profile => profile.Id,
                (activation, profile) => new ServerTransportActivationListItemDto(
                    activation.Id,
                    activation.ServerId,
                    activation.TransportProfileId,
                    profile.Name,
                    activation.Status,
                    activation.LocalPriority,
                    activation.RealityPublicKey,
                    activation.ActivatedAtUtc,
                    activation.CreatedAtUtc,
                    activation.UpdatedAtUtc,
                    activation.Version))
            .OrderByDescending(x => x.Status)
            .ThenBy(x => x.LocalPriority)
            .ThenBy(x => x.TransportProfileName)
            .ToListAsync(ct);

        return items;
    }
}
