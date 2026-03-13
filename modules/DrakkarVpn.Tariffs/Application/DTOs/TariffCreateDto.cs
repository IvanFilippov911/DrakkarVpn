namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;

public sealed record TariffCreateDto(
    string Name,
    TimeSpan Duration,
    decimal Price,
    int DefaultMaxDevices
);