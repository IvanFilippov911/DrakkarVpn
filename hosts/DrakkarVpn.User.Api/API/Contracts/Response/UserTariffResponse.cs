namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Tariffs;

/// <summary>
/// Public API contract for an active tariff in user-facing endpoints.
/// </summary>
public sealed record UserTariffResponse(
    Guid Id,
    string Name,
    decimal Price,
    TimeSpan Duration,
    int DefaultMaxDevices);
