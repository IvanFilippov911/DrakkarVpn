using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Mappers;

public static class AdminUserMappers
{
    public static AdminUserSummaryDetailDto ToAdminDto(this UserSummaryDetailDto dto)
        => new(
            UserId:      dto.UserId,
            TelegramId:  dto.TelegramId,
            Username:    dto.Username,
            CreatedAtUtc:dto.CreatedAtUtc,
            Status:      dto.Status,
            IsInternal:  dto.IsInternal,
            BanReason:   dto.BanReason,
            BannedAtUtc: dto.BannedAtUtc
        );
}