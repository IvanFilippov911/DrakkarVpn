namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Request;

public sealed record AdminBulkGrantSubscriptionsRequest(
    IReadOnlyList<Guid> UserIds,
    Guid TariffId,
    int? DeviceCount
);