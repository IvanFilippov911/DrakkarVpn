using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Users.Infrastructure.Infrastructure.rowDTOs;

public sealed class UserSubRow
{
    public AppUser User { get; init; } = default!;

    public Guid?          SubscriptionId          { get; init; }
    public DateTime?      SubscriptionEndUtc      { get; init; }
    public int            SubscriptionMaxDevices  { get; init; }
    public SubscriptionStatus? SubscriptionStatus { get; init; }
}