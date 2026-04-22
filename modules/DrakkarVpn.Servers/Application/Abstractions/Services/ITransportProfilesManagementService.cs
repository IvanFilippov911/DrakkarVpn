using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface ITransportProfilesManagementService
{
    Task<PagedResponseDto<TransportProfileDto>> GetPagedAsync(
        GetTransportProfilesFilterDto filter,
        CancellationToken ct);

    Task<Guid> CreateAsync(CreateTransportProfileDto dto, CancellationToken ct);

    Task<bool> UpdateAsync(
        Guid profileId,
        UpdateTransportProfileDto dto,
        CancellationToken ct);

    Task<bool> EnableAsync(Guid profileId, CancellationToken ct);
    Task<bool> DisableAsync(Guid profileId, CancellationToken ct);

    Task<DeleteTransportProfileResult> DeleteAsync(Guid profileId, CancellationToken ct);
}
