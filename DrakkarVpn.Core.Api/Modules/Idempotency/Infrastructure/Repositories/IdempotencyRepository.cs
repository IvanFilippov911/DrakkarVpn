using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Idempotency.Domain;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Idempotency.Infrastructure.Repositories;

public sealed class IdempotencyRepository : IIdempotencyRepository
{
    private readonly AppDbContext _db;

    public IdempotencyRepository(AppDbContext db) => _db = db;

    public async Task<IdempotencyKey?> FindAsync(string actorKey, string action, Guid requestId, CancellationToken ct)
    {
        return await _db.Set<IdempotencyKey>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ActorKey == actorKey &&
                x.Action == action &&
                x.RequestId == requestId, ct);
    }

    public async Task<IdempotencyKey> StartAsync(
        string actorKey,
        string action,
        Guid requestId,
        TimeSpan? ttl,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var entity = IdempotencyKey.Start(actorKey, action, requestId, now, ttl);

        _db.Set<IdempotencyKey>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task MarkSucceededAsync(Guid id, string? resultJson, CancellationToken ct)
    {
        var entity = await _db.Set<IdempotencyKey>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;

        entity.Succeed(resultJson, DateTime.UtcNow);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            
        }
    }

    public async Task MarkFailedAsync(Guid id, string error, CancellationToken ct)
    {
        var entity = await _db.Set<IdempotencyKey>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;

        entity.Fail(error, DateTime.UtcNow);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
           
        }
    }
}