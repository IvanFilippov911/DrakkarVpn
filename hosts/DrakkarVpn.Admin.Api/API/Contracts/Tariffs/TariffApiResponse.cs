namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Tariffs;

/// <summary>
/// API response contract for tariff in admin endpoints.
/// Mirrors TariffDto from shared tariffs.
/// </summary>
public sealed record TariffApiResponse(
    Guid Id,
    string Name,
    decimal Price,
    TimeSpan Duration,
    string Status,
    DateTime CreatedAt,
    int DefaultMaxDevices);

