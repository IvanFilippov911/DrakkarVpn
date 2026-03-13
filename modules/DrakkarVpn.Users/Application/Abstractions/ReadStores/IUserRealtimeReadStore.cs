using DrakkarVpn.Users.Application.DTOs;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IUserRealtimeReadStore
{
    Task<UserRealtimeDto?> GetRealtimeAsync(
        Guid userId,
        CancellationToken ct);
}