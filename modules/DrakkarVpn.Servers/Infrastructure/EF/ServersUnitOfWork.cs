using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Servers.Infrastructure.EF;

public sealed class ServersUnitOfWork : IServersUnitOfWork
{
    private readonly ServerDbContext _db;

    public ServersUnitOfWork(ServerDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await action(ct);
            await tx.CommitAsync(ct);
        });
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var result = await action(ct);
            await tx.CommitAsync(ct);
            return result;
        });
    }
}
