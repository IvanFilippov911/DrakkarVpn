namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record UserSummaryDto(
    DateTime? SubscriptionEndAtUtc,
    int ConnectedDevices,
    int? MaxDevices
);

