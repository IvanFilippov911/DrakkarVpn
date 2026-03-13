public sealed record ExpireSubscriptionsResultDto(
    IReadOnlyList<Guid> SucceededSubscriptionIds,
    IReadOnlyList<Guid> FailedSubscriptionIds)
{
    public static ExpireSubscriptionsResultDto Empty()
        => new(Array.Empty<Guid>(), Array.Empty<Guid>());

    public static ExpireSubscriptionsResultDto Succeeded(IReadOnlyCollection<Guid> succeeded)
        => new(succeeded.Distinct().ToArray(), Array.Empty<Guid>());

    public static ExpireSubscriptionsResultDto Partial(
        IReadOnlyCollection<Guid> succeeded,
        IReadOnlyCollection<Guid> failed)
        => new(succeeded.Distinct().ToArray(), failed.Distinct().ToArray());

    public static ExpireSubscriptionsResultDto Failed(IReadOnlyCollection<Guid> failed)
        => new(Array.Empty<Guid>(), failed.Distinct().ToArray());
}