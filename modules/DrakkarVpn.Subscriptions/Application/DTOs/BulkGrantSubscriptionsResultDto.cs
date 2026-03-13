namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record BulkGrantSubscriptionsResultDto(
    IReadOnlyList<Guid> Succeeded,
    IReadOnlyList<Guid> Failed,
    IReadOnlyList<BulkUserGrantFailureDetail> FailureDetails)
{
    public static BulkGrantSubscriptionsResultDto Empty()
        => new(Array.Empty<Guid>(), Array.Empty<Guid>(), Array.Empty<BulkUserGrantFailureDetail>());

    public static BulkGrantSubscriptionsResultDto TariffNotAvailable(
        IReadOnlyList<Guid> userIds,
        Guid tariffId)
        => new(
            Succeeded: Array.Empty<Guid>(),
            Failed: userIds,
            FailureDetails: userIds.Select(id =>
                new BulkUserGrantFailureDetail(id, "TariffNotAvailable", $"Tariff {tariffId} not available")
            ).ToList()
        );

    public static BulkGrantSubscriptionsResultDto Ok(
        IReadOnlyList<Guid> succeeded,
        IReadOnlyList<Guid> failed,
        IReadOnlyList<BulkUserGrantFailureDetail>? details = null)
        => new(
            Succeeded: succeeded,
            Failed: failed,
            FailureDetails: details ?? Array.Empty<BulkUserGrantFailureDetail>()
        );
}