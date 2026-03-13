namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed record BulkMarkUsersInternalRequest(
    IReadOnlyList<Guid> UserIds,
    bool IsInternal
);