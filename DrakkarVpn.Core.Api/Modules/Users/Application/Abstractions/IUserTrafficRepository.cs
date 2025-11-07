using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IUserTrafficRepository
{
    Task<Dictionary<Guid, UsersTrafficSummaryDto>> GetTrafficLast24hAsync(
        Guid serverId,
        Guid[] userIds,
        DateTime fromUtc,
        CancellationToken ct);
}