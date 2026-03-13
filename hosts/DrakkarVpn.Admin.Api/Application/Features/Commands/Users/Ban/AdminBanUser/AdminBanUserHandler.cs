using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Shared.Errors;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBanUser;

// TEMPORARY:
// Explicit cross-context orchestration with manual SaveChanges.
// Will be migrated to Outbox / Event-driven flow later.
public sealed class AdminBanUserHandler
    : IRequestHandler<AdminBanUserCommand>
{
    private readonly IUserModerationService _users;
    private readonly IPeerRevocationService _peerRevocationService;
    private readonly IUsersUnitOfWork _usersUow;
    private readonly IPeersUnitOfWork _peersUow;

    public AdminBanUserHandler(
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

    public async Task Handle(AdminBanUserCommand cmd, CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        // Step 1: ban user in Users context
        var ban = await _users.BanUsersBulkAsync(
            new[] { cmd.UserId },
            cmd.Reason,
            ct);

        if (ban.NotFoundUserIds.Count > 0)
            throw new KeyNotFoundException($"User {cmd.UserId} not found.");

        if (ban.FailedUserIds.Count > 0 || ban.SucceededUserIds.Count == 0)
            throw new InvalidOperationException($"Failed to ban user {cmd.UserId}.");

        await _usersUow.SaveChangesAsync(ct);

        // Step 2: revoke peers in Peers context
        var revoke = await _peerRevocationService.RevokeUsersPeersAsync(
            ban.SucceededUserIds,
            markerUtc,
            ct);

        if (revoke.SucceededUserIds.Count == 0)
            throw PeersRevokeFailedException.ForUser(
                cmd.UserId,
                revoked: 0,
                failed: revoke.FailedUserIds.Count);

        if (revoke.FailedUserIds.Count > 0)
            throw PeersRevokeFailedException.ForUser(
                cmd.UserId,
                revoked: revoke.SucceededUserIds.Count,
                failed: revoke.FailedUserIds.Count);

        await _peersUow.SaveChangesAsync(ct);
    }
}