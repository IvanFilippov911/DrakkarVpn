using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;

public sealed record BulkUserFailureDetail(
    Guid UserId,
    string Code,
    string Message,
    IReadOnlyList<BulkUserFailureItem>? Items = null
);