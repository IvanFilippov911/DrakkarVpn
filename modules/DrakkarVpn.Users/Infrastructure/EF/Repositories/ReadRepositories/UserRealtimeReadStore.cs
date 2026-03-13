using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs;
using DrakkarVpn.Users.Application.DTOs.Admin;
using DrakkarVpn.Users.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Users.Infrastructure.Repositories.ReadRepositories;

public sealed class UserRealtimeReadStore : IUserRealtimeReadStore
{
    private readonly UsersDbContext _db;

    public UserRealtimeReadStore(UsersDbContext db) => _db = db;

    public async Task<UserRealtimeDto?> GetRealtimeAsync(
        Guid userId,
        CancellationToken ct)
    {
        return await _db.UserRealtimeStats
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => new UserRealtimeDto(
                s.IsOnline,
                s.DeviceCount,
                s.SubscriptionMaxDevices,
                s.IsSubscriptionActive,
                s.SubscriptionEndUtc,
                s.Traffic24hBytes,
                s.UpdatedAtUtc,
                s.LastSeenUtc
            ))
            .SingleOrDefaultAsync(ct);
    }
}