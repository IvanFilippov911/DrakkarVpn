using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;

public interface ITariffQueryService
{
    Task<IReadOnlyList<TariffDto>> GetActiveAsync(CancellationToken ct);
    Task<IReadOnlyList<TariffDto>> GetActiveNonTrialAsync(CancellationToken ct);
    Task<TariffDto?> GetByIdAsync(Guid tariffId, CancellationToken ct);
    Task<TariffDto?> GetFirstActiveByKindAsync(TariffKind kind, CancellationToken ct);
}
