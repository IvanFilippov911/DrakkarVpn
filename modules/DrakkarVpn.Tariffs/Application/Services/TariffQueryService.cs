using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Mappers;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Services;

public sealed class TariffQueryService : ITariffQueryService
{
    private readonly ITariffRepository _repo;

    public TariffQueryService(ITariffRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<TariffDto>> GetActiveAsync(CancellationToken ct)
    {
        var tariffs = await _repo.GetAllActiveAsync(ct);
        return tariffs.Select(t => t.ToDto()).ToList();
    }

    public async Task<TariffDto?> GetByIdAsync(Guid tariffId, CancellationToken ct)
    {
        if (tariffId == Guid.Empty)
            return null;

        var tariff = await _repo.GetByIdAsync(new(tariffId), ct);
        return tariff?.ToDto();
    }
}