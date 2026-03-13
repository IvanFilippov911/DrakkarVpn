using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;

public interface ITariffAdminService
{
    Task<Guid> CreateAsync(TariffCreateDto dto, CancellationToken ct);
    Task EnableAsync(Guid tariffId, CancellationToken ct);
    Task DisableAsync(Guid tariffId, CancellationToken ct);
    Task UpdateAsync(Guid tariffId, TariffUpdateDto dto, CancellationToken ct);
}