using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Users.Application.DTOs.Admin;

public sealed record UserSummaryDetailDto(
    Guid       UserId,
    long       TelegramId,
    string?    Username,
    DateTime   CreatedAtUtc,
    UserStatus Status,
    bool       IsInternal,
    string?    BanReason,
    DateTime?  BannedAtUtc
);