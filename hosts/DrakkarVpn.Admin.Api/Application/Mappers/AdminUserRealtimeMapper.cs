using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;
using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Mappers;

public static class AdminUserRealtimeMapper
{
    public static AdminUserRealtimeDto ToAdminDto(this UserRealtimeDto src)
        => new(
            IsOnline:               src.IsOnline,
            DeviceCount:            src.DeviceCount,
            SubscriptionMaxDevices: src.SubscriptionMaxDevices,
            IsSubscriptionActive:   src.IsSubscriptionActive,
            SubscriptionEndUtc:     src.SubscriptionEndUtc,
            Traffic24hBytes:        src.Traffic24hBytes,
            UpdatedAtUtc:           src.UpdatedAtUtc,
            LastSeenUtc:            src.LastSeenUtc
        );
    
    public static AdminUserRealtimeDto ToAdminDtoOrDefault(this UserRealtimeDto? src, DateTime nowUtc)
        => src is null
            ? new AdminUserRealtimeDto(UpdatedAtUtc: nowUtc)
            : src.ToAdminDto();
}