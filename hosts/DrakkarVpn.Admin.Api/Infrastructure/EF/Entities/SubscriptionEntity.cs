

using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Infrastructure.EF.Entity;

public sealed class SubscriptionEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }

    public SubscriptionStatus Status { get; set; }
    public DateTime StatusUpdatedAtUtc { get; set; }

    public int MaxDevices { get; set; }
}