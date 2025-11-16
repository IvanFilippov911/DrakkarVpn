using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.Validation;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Domain;

public sealed class Tariff
{
    private Tariff() { } 

    private Tariff(
        TariffId id,
        string name,
        TimeSpan duration,
        decimal price,
        int defaultMaxDevices)
    {
        Id = id;
        Apply(name, duration, price, defaultMaxDevices);
        CreatedAt = DateTime.UtcNow;
        Status    = TariffStatus.Active;
    }

    public TariffId Id { get; }
    public string Name { get; private set; } = null!;
    public TimeSpan Duration { get; private set; }
    public decimal Price { get; private set; }
    public int DefaultMaxDevices { get; private set; }

    public TariffStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public static Tariff CreateNew(
        string name,
        TimeSpan duration,
        decimal price,
        int defaultMaxDevices) =>
        new(
            TariffId.New(),
            name,
            duration,
            price,
            defaultMaxDevices
        );

    public void Update(
        string name,
        decimal price,
        TimeSpan duration,
        int defaultMaxDevices)
    {
        Apply(name, duration, price, defaultMaxDevices);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Disable() => Status = TariffStatus.Disabled;
    public void Enable()  => Status = TariffStatus.Active;

    private void Apply(
        string name,
        TimeSpan duration,
        decimal price,
        int defaultMaxDevices)
    {
        TariffValidation.ValidateAndThrow(name, duration, price, defaultMaxDevices);

        Name              = name;
        Duration          = duration;
        Price             = price;
        DefaultMaxDevices = defaultMaxDevices;
    }
}