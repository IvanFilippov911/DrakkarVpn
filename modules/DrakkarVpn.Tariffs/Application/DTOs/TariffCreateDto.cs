using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;

public sealed record TariffCreateDto(
    string Name,
    TimeSpan Duration,
    decimal Price,
    int DefaultMaxDevices,
    TariffKind Kind = TariffKind.Standard
);