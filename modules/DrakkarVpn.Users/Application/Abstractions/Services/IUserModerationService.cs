using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Users.Application.Abstractions.Servers;

public interface IUserModerationService
{
    Task<BulkUsersOperationResult> BanUsersBulkAsync(
        IReadOnlyCollection<Guid> userIds, string? reason, CancellationToken ct);

    Task<BulkUsersOperationResult> UnbanUsersBulkAsync(
        IReadOnlyCollection<Guid> userIds, CancellationToken ct);

    Task<BulkUsersOperationResult> MarkInternalBulkAsync(
        IReadOnlyCollection<Guid> userIds, bool isInternal, CancellationToken ct);
}