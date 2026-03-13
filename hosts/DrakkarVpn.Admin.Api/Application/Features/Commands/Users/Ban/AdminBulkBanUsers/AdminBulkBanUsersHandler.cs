using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkBanUsers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using DrakkarVpn.Users.Application.DTOs.Admin;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.Bulk.AdminBulkBanUsers;

// TEMPORARY:
// Explicit cross-context orchestration with manual SaveChanges.
// Will be migrated to Outbox / Event-driven flow later.
public sealed class AdminBulkBanUsersHandler
    : IRequestHandler<AdminBulkBanUsersCommand, BulkUsersOperationResponse>
{
    private readonly IUserModerationService _users;
    private readonly IPeerRevocationService _peerRevocationService;
    private readonly IUsersUnitOfWork _usersUow;
    private readonly IPeersUnitOfWork _peersUow;

    public AdminBulkBanUsersHandler(
        IUserModerationService users,
        IPeerRevocationService peerRevocationService,
        IUsersUnitOfWork usersUow,
        IPeersUnitOfWork peersUow)
    {
        _users = users;
        _peerRevocationService = peerRevocationService;
        _usersUow = usersUow;
        _peersUow = peersUow;
    }

    public async Task<BulkUsersOperationResponse> Handle(AdminBulkBanUsersCommand cmd, CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        // Step 1: changes in context A (Users) → SaveChangesAsync(Users)
        var ban = await _users.BanUsersBulkAsync(cmd.UserIds, cmd.Reason, ct);

        if (ban.SucceededUserIds.Count == 0)
            return ban.ToResponse();

        await _usersUow.SaveChangesAsync(ct);

        // Step 2: changes in context B (Peers) → SaveChangesAsync(Peers)
        var revoke = await _peerRevocationService.RevokeUsersPeersAsync(ban.SucceededUserIds, markerUtc, ct);
        await _peersUow.SaveChangesAsync(ct);

        if (revoke.FailedUserIds.Count == 0)
            return ban.ToResponse();

        var failureDetails = revoke.FailureDetails
            .GroupBy(x => x.UserId)
            .Select(g => new BulkUserFailureDetail(
                UserId: g.Key,
                Code: "PeersRevokeFailed",
                Message: $"Failed to revoke {g.Count()} peers",
                Items: g.Select(x => new BulkUserFailureItem(
                    PeerId: x.PeerId,
                    ServerId: x.ServerId,
                    Code: x.Reason
                )).ToList()
            ))
            .ToList();
        
        
        return ban.ToResponse() with
        {
            FailureDetails = failureDetails
        };
    }
}