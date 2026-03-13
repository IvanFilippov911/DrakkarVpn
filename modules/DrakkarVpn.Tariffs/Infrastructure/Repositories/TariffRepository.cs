using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.Repositories;

public sealed class TariffRepository : ITariffRepository
{
    private readonly TariffsDbContext _db;
    public TariffRepository(TariffsDbContext db) => _db = db;

    public Task<Tariff?> GetByIdAsync(TariffId id, CancellationToken ct = default) =>
        _db.Tariffs.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<IReadOnlyList<Tariff>> GetAllActiveAsync(CancellationToken ct = default) =>
        _db.Tariffs.Where(x => x.Status == TariffStatus.Active).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<Tariff>)t.Result, ct);

    public Task AddAsync(Tariff tariff, CancellationToken ct = default)
    {
        _db.Tariffs.AddAsync(tariff, ct);
        return Task.CompletedTask;
    }
        
}