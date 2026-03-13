using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;

namespace DrakkarVpn.Tariffs.Infrastructure.EF;

public sealed class TariffsUnitOfWork : ITariffsUnitOfWork
{
    private readonly TariffsDbContext _db;

    public TariffsUnitOfWork(TariffsDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}