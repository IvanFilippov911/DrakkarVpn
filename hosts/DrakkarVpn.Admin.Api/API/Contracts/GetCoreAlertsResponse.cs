using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed record GetCoreAlertsResponse(
    IReadOnlyList<CoreAlertListItemDto> Items,
    int Page,
    int PageSize,
    int Total
);