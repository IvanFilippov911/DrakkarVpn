using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;

public sealed record BulkGrantSubscriptionsResponse(
    IReadOnlyList<Guid> Succeeded,
    IReadOnlyList<Guid> NotFound,
    IReadOnlyList<Guid> Failed,
    IReadOnlyList<BulkUserGrantFailureDetail>? FailureDetails)
{
    public static BulkGrantSubscriptionsResponse Empty()
        => new(Array.Empty<Guid>(), Array.Empty<Guid>(), Array.Empty<Guid>(), null);

    public static BulkGrantSubscriptionsResponse OnlyNotFound(IReadOnlyList<Guid> notFound)
        => new(Array.Empty<Guid>(), notFound, Array.Empty<Guid>(), null);

    public static BulkGrantSubscriptionsResponse From(
        BulkGrantSubscriptionsResultDto res,
        IReadOnlyList<Guid> notFound)
        => new(
            Succeeded: res.Succeeded,
            NotFound:  notFound,
            Failed:    res.Failed,
            FailureDetails: res.FailureDetails.Count == 0 ? null : res.FailureDetails
        );
}