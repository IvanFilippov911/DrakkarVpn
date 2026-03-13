using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Observability.Application.Abstracts.Services;

public interface ICoreHealthQueryService
{
    Task<CoreHealthDto> GetSnapshotAsync(CancellationToken ct);

    Task<IReadOnlyList<CoreHealthHistoryPointDto>> GetHistoryAsync(
        int? limit,
        CancellationToken ct);
}