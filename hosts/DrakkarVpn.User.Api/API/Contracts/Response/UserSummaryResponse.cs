namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Response;

public sealed record UserSummaryResponse(
    DateTime? SubscriptionEndAtUtc,
    int ConnectedDevices,
    int? MaxDevices
);

