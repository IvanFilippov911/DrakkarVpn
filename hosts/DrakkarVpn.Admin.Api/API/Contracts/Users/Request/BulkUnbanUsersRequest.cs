namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed record BulkUnbanUsersRequest(
    IReadOnlyCollection<Guid> UserIds
);