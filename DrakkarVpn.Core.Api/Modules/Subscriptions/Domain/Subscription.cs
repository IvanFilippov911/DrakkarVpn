using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;

public sealed class Subscription
{
    private Subscription() { }

    private Subscription(
        SubscriptionId id,
        Guid userId,
        TariffId tariffId,
        DateTime startAt,
        DateTime endAt)
    {
        Id = id;
        UserId = userId;
        TariffId = tariffId;
        StartAt = startAt;
        EndAt = endAt;
        Status = SubscriptionStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public SubscriptionId Id { get; private set; }
    public Guid UserId { get; private set; }
    public TariffId TariffId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Subscription CreateNew(Guid userId, TariffId tariffId, DateTime startAt, DateTime endAt) =>
        new(SubscriptionId.New(), userId, tariffId, startAt, endAt);

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
}