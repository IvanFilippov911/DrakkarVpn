using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Infrastructure.EF.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class TransportProfileReadRepository : ITransportProfileReadRepository
{
    private readonly ServerDbContext _db;

    public TransportProfileReadRepository(ServerDbContext db)
        => _db = db;

    public async Task<(IReadOnlyList<TransportProfileDto> Items, int Total)> GetPagedAsync(
        GetTransportProfilesFilterDto filter,
        CancellationToken ct)
    {
        var page = filter.Page <= 0 ? 1 : filter.Page;
        var pageSize = Math.Clamp(filter.PageSize <= 0 ? 25 : filter.PageSize, 1, 200);

        var query = _db.ServerTransportProfiles
            .AsNoTracking()
            .FilterBySearch(filter.Search)
            .FilterByIsEnabled(filter.IsEnabled)
            .FilterByTransportType(filter.TransportType);

        var total = await query.CountAsync(ct);
        if (total == 0)
            return (Array.Empty<TransportProfileDto>(), 0);

        var items = await query
            .ApplySorting(filter.SortBy, filter.SortDirection)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TransportProfileDto(
                x.Id,
                x.Name,
                x.TransportType,
                x.SecurityType,
                x.RealitySni,
                x.RealityShortId,
                x.RealityFingerprint,
                x.RealityDest,
                x.GrpcServiceName,
                x.GrpcAuthority,
                x.GlobalPriority,
                x.IsEnabled,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                x.Version))
            .ToListAsync(ct);

        return (items, total);
    }
}
