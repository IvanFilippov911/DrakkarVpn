namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

public sealed record VpnAccessContextDto(
    Guid UserId,
    string DeviceId,
    DateTime NowUtc,
    Guid SubscriptionId,
    int MaxDevices);

