using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Infrastructure.Repositories;

public sealed class TariffRepository : ITariffRepository
{
    private readonly AppDbContext _db;

    public TariffRepository(AppDbContext db) => _db = db;

    public async Task<Tariff?> GetByIdAsync(TariffId id, CancellationToken ct = default) =>
        await _db.Tariffs.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Tariff>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.Tariffs.Where(x => x.Status == TariffStatus.Active).ToListAsync(ct);

    public async Task AddAsync(Tariff tariff, CancellationToken ct = default) =>
        await _db.Tariffs.AddAsync(tariff, ct);
}