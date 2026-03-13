using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Infrastructure.EF.Entity;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories.rowDTOs;

public sealed class UserWithStatsRow
{
    public AppUser User { get; init; } = null!;
    public UserRealtimeStats? Stats { get; init; }
}