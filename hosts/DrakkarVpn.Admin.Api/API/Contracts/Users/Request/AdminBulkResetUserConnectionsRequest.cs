namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;


public sealed record AdminBulkResetUserConnectionsRequest(
    IReadOnlyList<Guid> UserIds
);