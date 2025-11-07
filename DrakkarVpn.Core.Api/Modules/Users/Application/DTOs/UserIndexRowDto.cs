using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record UserIndexRowDto(
    Guid       UserId,
    long       Telegram,
    DateTime   CreatedAtUtc,
    UserStatus Status,
    int        OnlinePeersCount,
    int        DeviceCount,
    DateTime?  LastSeenUtc
)
{
    public bool IsOnline => OnlinePeersCount > 0;
}