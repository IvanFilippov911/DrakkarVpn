using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;

namespace DrakkarVpn.Shared.Tariffs;

public sealed record TariffDto(
    Guid Id,
    string Name,
    decimal Price,
    TimeSpan Duration,
    TariffStatus Status,
    DateTime CreatedAt,
    int DefaultMaxDevices,
    TariffKind Kind
);