using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetGlobalCoreAlertsSummary;

public sealed class GetGlobalCoreAlertsSummaryHandler
    : IRequestHandler<GetGlobalCoreAlertsSummaryQuery, CoreAlertsGlobalSummaryDto>
{
    private readonly ICoreAlertsQueryService _queryService;

    public GetGlobalCoreAlertsSummaryHandler(ICoreAlertsQueryService queryService)
    {
        _queryService = queryService;
    }

    public Task<CoreAlertsGlobalSummaryDto> Handle(
        GetGlobalCoreAlertsSummaryQuery q,
        CancellationToken ct)
        => _queryService.GetGlobalOpenSummaryAsync(ct);
}