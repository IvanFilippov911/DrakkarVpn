using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Services.Alerts.CoreHealth;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.HealthSystem.GetCoreHealth;

public sealed class GetCoreHealthHandler
    : IRequestHandler<GetCoreHealthQuery, CoreHealthDto>
{
    private readonly ICoreHealthQueryService   _coreHealth;
    private readonly ICoreHealthAlertFactory   _alertsFactory;
    private readonly ICoreAlertService         _alertService;

    public GetCoreHealthHandler(
        ICoreHealthQueryService coreHealth,
        ICoreHealthAlertFactory alertsFactory,
        ICoreAlertService alertService)
    {
        _coreHealth     = coreHealth;
        _alertsFactory  = alertsFactory;
        _alertService   = alertService;
    }

    public async Task<CoreHealthDto> Handle(GetCoreHealthQuery _, CancellationToken ct)
    {
        var dto = await _coreHealth.GetSnapshotAsync(ct);
        
        if (dto.TotalRequests == 0)
            return dto;

        var alerts = _alertsFactory.Build(dto);
        if (alerts.Count > 0)
            await _alertService.CreateBatchAsync(alerts, ct);

        return dto;
    }
}