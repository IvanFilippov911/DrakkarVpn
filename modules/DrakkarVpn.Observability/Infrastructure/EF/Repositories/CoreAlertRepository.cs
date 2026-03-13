using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Extensions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using DrakkarVpn.Observability.Application.DTOs.Alerts;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Observability.Infrastructure.EF.Repositories;

public sealed class CoreAlertRepository : ICoreAlertRepository
{
    private readonly ObservabilityDbContext _db;

    public CoreAlertRepository(ObservabilityDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(CoreAlert alert, CancellationToken ct)
    {
        await _db.CoreAlerts.AddAsync(alert, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<CoreAlert>> GetPagedAsync(
        int page,
        int pageSize,
        bool? isResolved,
        string? source,
        string? severity,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct)
    {
        var q = _db.CoreAlerts.AsQueryable()
            .ApplyFilters(isResolved, source, severity, fromUtc, toUtc);

        return await q
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        bool? isResolved,
        string? source,
        string? severity,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct)
    {
        var q = _db.CoreAlerts.AsQueryable()
            .ApplyFilters(isResolved, source, severity, fromUtc, toUtc);

        return await q.CountAsync(ct);
    }

    public async Task<bool> MarkResolvedAsync(
        Guid id,
        CoreAlertResolutionType resolutionType,
        string? resolutionNote,
        Guid? resolvedByAdminId,
        CancellationToken ct)
    {
        var alert = await _db.CoreAlerts.FindAsync(new object?[] { id }, ct);
        if (alert is null)
            return false;

        if (!alert.IsResolved)
        {
            alert.IsResolved       = true;
            alert.ResolvedAtUtc    = DateTime.UtcNow;
            alert.ResolutionType   = resolutionType;
            alert.ResolutionNote   = resolutionNote;
            alert.ResolvedByAdminId = resolvedByAdminId;

            await _db.SaveChangesAsync(ct);
            return true;
        }
        
        alert.ResolutionType    = resolutionType;
        alert.ResolutionNote    = resolutionNote;
        alert.ResolvedByAdminId = resolvedByAdminId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var alert = await _db.CoreAlerts.FindAsync(new object?[] { id }, ct);
        if (alert is null)
            return false;

        _db.CoreAlerts.Remove(alert);
        await _db.SaveChangesAsync(ct);
        return true;
    }
    
    public async Task InsertManyAsync(
        IEnumerable<CoreAlert> entities,
        CancellationToken ct)
    {
        await _db.CoreAlerts.AddRangeAsync(entities, ct);
        await _db.SaveChangesAsync(ct);
    }
    
    public async Task<CoreAlertsGlobalSummaryDto> GetGlobalOpenSummaryAsync(
        CancellationToken ct)
    {
        var q = _db.CoreAlerts
            .AsNoTracking()
            .Where(a => !a.IsResolved);

        var groups = await q
            .GroupBy(a => a.Severity)
            .Select(g => new
            {
                Severity = g.Key,
                Count    = g.Count()
            })
            .ToListAsync(ct);

        int critical = groups.FirstOrDefault(x => x.Severity == "Critical")?.Count ?? 0;
        int warning  = groups.FirstOrDefault(x => x.Severity == "Warning")?.Count ?? 0;
        int info     = groups.FirstOrDefault(x => x.Severity == "Info")?.Count ?? 0;

        var total = await q.CountAsync(ct); 

        return new CoreAlertsGlobalSummaryDto(
            TotalOpen: total,
            Critical:  critical,
            Warning:   warning,
            Info:      info
        );
    }
    
    public async Task<IReadOnlyList<UserAlertDto>> GetLastAlertsAsync(
        Guid userId,
        int  take,
        CancellationToken ct)
    {
        return await _db.CoreAlerts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Take(take)
            .Select(a => new UserAlertDto(
                a.Id,
                a.CreatedAtUtc,
                a.IsResolved,
                a.Severity,
                a.Title,
                a.Message
            ))
            .ToListAsync(ct);
    }
}