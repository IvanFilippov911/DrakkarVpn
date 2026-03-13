using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Observability.Application.Abstracts.Services;

public interface ICoreErrorEventsQueryService
{
    Task<PagedResponseDto<CoreErrorEventListItemDto>> GetPagedAsync(GetCoreErrorEventsDto q, CancellationToken ct);
}