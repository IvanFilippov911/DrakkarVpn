using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;

public interface ITariffRepository
{
    Task<Tariff?> GetByIdAsync(TariffId id, CancellationToken ct = default);
    Task<IReadOnlyList<Tariff>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Tariff tariff, CancellationToken ct = default);
}