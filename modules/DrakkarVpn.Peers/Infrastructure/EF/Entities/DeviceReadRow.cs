using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

[Keyless]
public sealed class DeviceReadRow
{
    public string DeviceId { get; set; } = default!;
    public Guid UserId { get; set; }
    public short Status { get; set; } 
}