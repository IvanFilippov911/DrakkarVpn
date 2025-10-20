using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;

public sealed class Subscription
{
    private Subscription() { }

    private Subscription(
        SubscriptionId id,
        Guid userId,
        DateTime startAt,
        DateTime endAt,
        int maxDevices)
    {
        Id = id;
        UserId = userId;
        StartAt = startAt;
        EndAt = endAt;
        Status = SubscriptionStatus.Active;
        CreatedAt = DateTime.UtcNow;
        MaxDevices = maxDevices;
    }

    public SubscriptionId Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public int MaxDevices { get; private set; } 

    public static Subscription CreateNew(Guid userId, DateTime startAt, DateTime endAt, int maxDevices = 1)
    {
        if (maxDevices < 1) throw new ArgumentOutOfRangeException(nameof(maxDevices));
        var sub = new Subscription(SubscriptionId.New(), userId, startAt, endAt, maxDevices);
        sub.Status = SubscriptionStatus.Active;
        return sub;
    }

    public void Renew(DateTime newEndAt)
    {
        if (newEndAt <= EndAt)
            throw new InvalidOperationException("New end date must be later than current end date.");

        EndAt = newEndAt;
        Status = SubscriptionStatus.Active;
    }

    public bool IsActive() =>
        Status == SubscriptionStatus.Active && EndAt > DateTime.UtcNow;

    public void Expire() => Status = SubscriptionStatus.Expired;
    
    public void ChangeMaxDevices(int newMaxDevices)
    {
        if (newMaxDevices < 1) throw new ArgumentOutOfRangeException(nameof(newMaxDevices));
        MaxDevices = newMaxDevices;
    }
}