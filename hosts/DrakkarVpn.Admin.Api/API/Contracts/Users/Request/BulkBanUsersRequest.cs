namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed record BulkBanUsersRequest(
    IReadOnlyList<Guid> UserIds,
    string? Reason
);