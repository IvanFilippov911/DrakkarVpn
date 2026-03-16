namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Tariffs;

/// <summary>
/// API request contract for updating a tariff.
/// </summary>
public sealed record UpdateTariffApiRequest(
    string Name,
    int DurationDays,
    decimal Price,
    int DefaultMaxDevices);

