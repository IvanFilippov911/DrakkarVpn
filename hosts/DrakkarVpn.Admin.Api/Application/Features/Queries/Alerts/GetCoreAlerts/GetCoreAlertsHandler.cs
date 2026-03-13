using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreAlerts;

public sealed class GetCoreAlertsHandler
    : IRequestHandler<GetCoreAlertsQuery, PagedResponseDto<CoreAlertListItemDto>>
{
    private readonly ICoreAlertsQueryService _service;

    public GetCoreAlertsHandler(ICoreAlertsQueryService service)
        => _service = service;

    public Task<PagedResponseDto<CoreAlertListItemDto>> Handle(GetCoreAlertsQuery q, CancellationToken ct)
    {
        var dto = new CoreAlertsQueryDto(
            Page:       q.Page,
            PageSize:   q.PageSize,
            IsResolved: q.IsResolved,
            Source:     q.Source,
            Severity:   q.Severity,
            FromUtc:    q.FromUtc,
            ToUtc:      q.ToUtc
        );

        return _service.GetPagedAsync(dto, ct);
    }
}