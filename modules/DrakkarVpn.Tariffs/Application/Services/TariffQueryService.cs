using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Mappers;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
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

    public async Task<IReadOnlyList<TariffDto>> GetActiveNonTrialAsync(CancellationToken ct)
    {
        var tariffs = await _repo.GetAllActiveAsync(ct);
        return tariffs
            .Where(t => t.Kind != TariffKind.Trial)
            .Select(t => t.ToDto())
            .ToList();
    }

    public async Task<TariffDto?> GetByIdAsync(Guid tariffId, CancellationToken ct)
    {
        if (tariffId == Guid.Empty)
            return null;

        var tariff = await _repo.GetByIdAsync(new(tariffId), ct);
        return tariff?.ToDto();
    }

    public async Task<TariffDto?> GetFirstActiveByKindAsync(TariffKind kind, CancellationToken ct)
    {
        var tariff = await _repo.GetFirstActiveByKindAsync(kind, ct);
        return tariff?.ToDto();
    }
}