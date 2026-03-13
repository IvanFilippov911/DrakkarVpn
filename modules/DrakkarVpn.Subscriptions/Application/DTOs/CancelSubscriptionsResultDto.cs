namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record CancelSubscriptionsResultDto(
    IReadOnlyList<Guid> Succeeded,
    IReadOnlyList<Guid> Failed)
{
    public static CancelSubscriptionsResultDto Empty()
        => new(Array.Empty<Guid>(), Array.Empty<Guid>());

    public static CancelSubscriptionsResultDto AllFailed(IReadOnlyList<Guid> ids)
        => new(Array.Empty<Guid>(), ids);

    public static CancelSubscriptionsResultDto Partial(
        IReadOnlyList<Guid> ok,
        IReadOnlyList<Guid> failed)
        => new(ok, failed);

    public static CancelSubscriptionsResultDto AllSucceeded(IReadOnlyList<Guid> ok)
        => new(ok, Array.Empty<Guid>());
}