namespace DrakkarVpn.Users.Application.DTOs.Admin;

public sealed record BulkUserFailureItem(
    Guid? PeerId,
    Guid? ServerId,
    string Code,
    string? Message = null
);