using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;

public interface ITariffQueryService
{
    Task<IReadOnlyList<TariffDto>> GetActiveAsync(CancellationToken ct);
    Task<TariffDto?> GetByIdAsync(Guid tariffId, CancellationToken ct);
}
