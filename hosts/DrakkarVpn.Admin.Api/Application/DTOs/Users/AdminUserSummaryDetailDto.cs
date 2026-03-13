using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;

public sealed record AdminUserSummaryDetailDto(
    Guid       UserId,
    long       TelegramId,
    string?    Username,
    DateTime   CreatedAtUtc,
    UserStatus Status,
    bool       IsInternal,
    string?    BanReason,
    DateTime?  BannedAtUtc
);