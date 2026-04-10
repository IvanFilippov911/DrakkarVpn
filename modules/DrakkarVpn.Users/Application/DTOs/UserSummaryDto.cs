using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record UserSummaryDto(
    Guid Id,
    long? Telegram,
    DateTime CreatedAtUtc,
    UserStatus Status,
    bool IsOnline,
    int DeviceCount,
    DateTime?  LastSeenUtc,
    string? Username
);