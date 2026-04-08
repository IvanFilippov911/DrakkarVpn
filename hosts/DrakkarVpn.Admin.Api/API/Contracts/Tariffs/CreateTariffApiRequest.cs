using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Tariffs;

/// <summary>
/// API request contract for creating a tariff.
/// </summary>
public sealed record CreateTariffApiRequest(
    string Name,
    int DurationDays,
    decimal Price,
    int DefaultMaxDevices,
    TariffKind Kind = TariffKind.Standard);

