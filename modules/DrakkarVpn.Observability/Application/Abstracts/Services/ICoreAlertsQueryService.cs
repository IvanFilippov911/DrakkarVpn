using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Observability.Application.DTOs.Alerts;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Observability.Application.Abstracts.Services;

public interface ICoreAlertsQueryService
{
    Task<PagedResponseDto<CoreAlertListItemDto>> GetPagedAsync(CoreAlertsQueryDto q, CancellationToken ct);
    Task<CoreAlertsGlobalSummaryDto> GetGlobalOpenSummaryAsync(CancellationToken ct);
    Task<IReadOnlyList<UserAlertDto>> GetUserLastAlertsAsync(
        Guid userId,
        int take,
        CancellationToken ct);
}