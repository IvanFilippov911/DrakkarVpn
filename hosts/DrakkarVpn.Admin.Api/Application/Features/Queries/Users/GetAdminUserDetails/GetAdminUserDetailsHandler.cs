using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminUserDetails;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Mappers;
using DrakkarVpn.Observability.Application.Abstracts.Repositories;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.Users.GetAdminUserDetails;

public sealed class GetAdminUserDetailsHandler
    : IRequestHandler<GetAdminUserDetailsQuery, AdminUserDetailsDto>
{
    private readonly IAppUserReadStore   _summaryRead;
    private readonly IUserRealtimeReadStore  _realtimeRead;
    private readonly IUserDevicesReadStore   _devicesRead;
    private readonly ICoreAlertsQueryService    _alertsRead;

    private const int AlertsLimit = 20;

    public GetAdminUserDetailsHandler(
        IAppUserReadStore  summaryRead,
        IUserRealtimeReadStore realtimeRead,
        IUserDevicesReadStore  devicesRead,
        ICoreAlertsQueryService   alertsRead)
    {
        _summaryRead  = summaryRead;
        _realtimeRead = realtimeRead;
        _devicesRead  = devicesRead;
        _alertsRead   = alertsRead;
    }

    public async Task<AdminUserDetailsDto> Handle(
        GetAdminUserDetailsQuery request,
        CancellationToken ct)
    {
        var userSummaryDto = await _summaryRead.GetSummaryAsync(request.UserId, ct);
        if (userSummaryDto is null)
            throw new KeyNotFoundException($"User {request.UserId} not found");
        
        var userAdminSummmaryDto = userSummaryDto.ToAdminDto();
        
        var nowUtc = DateTime.UtcNow;

        var realtimeUserDto = await _realtimeRead.GetRealtimeAsync(request.UserId, ct);
        var realtimeAdminUserDto = realtimeUserDto.ToAdminDtoOrDefault(nowUtc);
        

        var devices = await _devicesRead.GetDevicesAsync(request.UserId, ct);

        var alerts  = await _alertsRead.GetUserLastAlertsAsync(request.UserId, AlertsLimit, ct);
        var adminAlerts = alerts.Select(a => new AdminUserAlertDto(
            a.Id,
            a.CreatedAtUtc,
            a.IsResolved,
            a.Severity,
            a.Title,
            a.Message
        )).ToList();
        
        return new AdminUserDetailsDto(
            User:     userAdminSummmaryDto,
            Realtime: realtimeAdminUserDto,
            Devices:  devices,
            Alerts:   adminAlerts
        );
    }
}