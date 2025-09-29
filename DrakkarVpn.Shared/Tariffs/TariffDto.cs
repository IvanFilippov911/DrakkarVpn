namespace DrakkarVpn.Shared.Tariffs;

public sealed record TariffDto(
    Guid Id,
    string Name,
    decimal Price,
    int DurationDays,
    string Status,
    DateTime CreatedAt
);