using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;

public sealed class Subscription
{
    private Subscription() { }

    private Subscription(
        Guid id,
        Guid userId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        int maxDevices,
        DateTime statusUpdatedAtUtc)
    {
        Id = id;
        UserId = userId;
        StartAt = EnsureUtc(startAtUtc);
        EndAt   = EnsureUtc(endAtUtc);

        if (EndAt <= StartAt)
            throw new ArgumentOutOfRangeException(nameof(endAtUtc), "EndAt must be > StartAt");

        if (maxDevices < 1)
            throw new ArgumentOutOfRangeException(nameof(maxDevices));

        Status = SubscriptionStatus.Active;
        MaxDevices = maxDevices;

        StatusUpdatedAtUtc = EnsureUtc(statusUpdatedAtUtc);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StatusUpdatedAtUtc { get; private set; }
    public int MaxDevices { get; private set; }

    public static Subscription CreateNew(
        Guid userId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        int maxDevices)
    {
        return new Subscription(
            Guid.NewGuid(),
            userId,
            startAtUtc,
            endAtUtc,
            maxDevices,
            statusUpdatedAtUtc: startAtUtc);
    }
    
    public static Subscription CreateNewForBulk(
        Guid userId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        int maxDevices,
        DateTime markerUtc)
    {
        return new Subscription(
            Guid.NewGuid(),
            userId,
            startAtUtc,
            endAtUtc,
            maxDevices,
            statusUpdatedAtUtc: markerUtc);
    }

    public void Renew(DateTime newEndAtUtc, DateTime nowUtc)
    {
        nowUtc    = EnsureUtc(nowUtc);
        newEndAtUtc = EnsureUtc(newEndAtUtc);

        if (newEndAtUtc <= nowUtc)
            throw new ArgumentOutOfRangeException(nameof(newEndAtUtc), "NewEndAt must be > nowUtc");

        EndAt = newEndAtUtc;
        Status = SubscriptionStatus.Active;
        StatusUpdatedAtUtc = nowUtc;
    }

    public bool IsActive(DateTime nowUtc)
    {
        nowUtc = EnsureUtc(nowUtc);
        return Status == SubscriptionStatus.Active && EndAt > nowUtc;
    }
    
    public void UpdateMaxDevices(int maxDevices)
    {
        if (maxDevices <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxDevices));

        MaxDevices = maxDevices;
    }

    public void Cancel(DateTime nowUtc)
    {
        nowUtc = EnsureUtc(nowUtc);

        if (Status == SubscriptionStatus.Cancelled)
            return;

        Status = SubscriptionStatus.Cancelled;
        StatusUpdatedAtUtc = nowUtc;

        if (EndAt > nowUtc)
            EndAt = nowUtc;
    }

    private static DateTime EnsureUtc(DateTime t)
    {
        if (t.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Timestamp must be UTC", nameof(t));
        return t;
    }
}