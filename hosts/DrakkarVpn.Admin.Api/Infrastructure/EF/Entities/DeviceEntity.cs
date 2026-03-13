using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions;

namespace DrakkarVpn.Users.Infrastructure.EF.Entity;

public sealed class DeviceEntity
{
    public string DeviceId { get; set; } = default!;
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public string? Platform { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastSeenUtc { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime? StatusUpdatedAtUtc { get; set; }
}