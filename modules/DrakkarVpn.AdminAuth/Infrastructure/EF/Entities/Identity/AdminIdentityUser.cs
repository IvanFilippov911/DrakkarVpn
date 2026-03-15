using Microsoft.AspNetCore.Identity;

namespace DrakkarVpn.AdminAuth.Infrastructure.Identity;

public sealed class AdminIdentityUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
}
