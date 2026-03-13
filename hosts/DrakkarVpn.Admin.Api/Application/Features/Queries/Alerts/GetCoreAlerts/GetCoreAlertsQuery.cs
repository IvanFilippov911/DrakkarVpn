using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreAlerts;

public sealed record GetCoreAlertsQuery(
    int Page,
    int PageSize,
    bool? IsResolved,
    string? Source,
    string? Severity,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IRequest<PagedResponseDto<CoreAlertListItemDto>>;