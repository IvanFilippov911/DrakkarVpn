using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;

public sealed record AdminUserDetailsDto(
    AdminUserSummaryDetailDto      User,
    AdminUserRealtimeDto     Realtime,
    IReadOnlyList<UserDeviceShortDto> Devices,
    IReadOnlyList<AdminUserAlertDto>       Alerts
);