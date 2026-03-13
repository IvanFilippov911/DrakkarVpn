using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Users.Infrastructure.EF.Entity;

public sealed class UserRealtimeStats
{
    public Guid UserId { get; set; }

    public DateTime? SubscriptionEndUtc { get; set; }
    public bool IsSubscriptionActive { get; set; }
    public SubscriptionStatus? LastSubscriptionStatus { get; set; }

    public int SubscriptionMaxDevices { get; set; }

    public int DeviceCount { get; set; }
    public DateTime? LastSeenUtc { get; set; }
    public bool IsOnline { get; set; }

    public long Traffic24hBytes { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}