using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Extensions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using DrakkarVpn.Observability.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Repository;

public sealed class CoreErrorEventRepository : ICoreErrorEventRepository
{
    private readonly ObservabilityDbContext _db;

    public CoreErrorEventRepository(ObservabilityDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(CoreErrorEvent evt, CancellationToken ct)
    {
        await _db.CoreErrorEvents.AddAsync(evt, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<CoreErrorEvent>> GetPagedAsync(
    int page,
    int pageSize,
    string? area,
    string? errorType,
    string? command,
    string? domainCode,
    string? userId,
    string? telegramId,
    string? search,
    DateTime? fromUtc,
    DateTime? toUtc,
    CancellationToken ct)
    {
        var q = _db.CoreErrorEvents
            .AsQueryable()
            .ApplyFilters(area, errorType, command, domainCode, userId, telegramId, search, fromUtc, toUtc);

        return await q
            .OrderByDescending(x => x.TimestampUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        string? area,
        string? errorType,
        string? command,
        string? domainCode,
        string? userId,
        string? telegramId,
        string? search,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct)
    {
        var q = _db.CoreErrorEvents
            .AsQueryable()
            .ApplyFilters(area, errorType, command, domainCode, userId, telegramId, search, fromUtc, toUtc);
        
        return await q.CountAsync(ct);
    }

    
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var e = await _db.CoreErrorEvents.FindAsync(new object?[] { id }, ct);
        if (e is null)
            return false;

        _db.CoreErrorEvents.Remove(e);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}