using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Domain;

public sealed class Tariff
{
    private Tariff() { }

    private Tariff(
        TariffId id,
        string name,
        TimeSpan duration,
        decimal price)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Duration = duration;
        Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
        CreatedAt = DateTime.UtcNow;
        Status = TariffStatus.Active;
    }

    public TariffId Id { get; }
    public string Name { get; private set; }
    public TimeSpan Duration { get; private set; }
    public decimal Price { get; private set; }
    public TariffStatus Status { get; private set; }
    public DateTime CreatedAt { get; }

    public static Tariff CreateNew(
        string name,
        TimeSpan duration,
        decimal price) =>
        new(TariffId.New(), name, duration, price);

    public void Disable() => Status = TariffStatus.Disabled;
    public void Enable()  => Status = TariffStatus.Active;
    
    public void Update(string name, decimal price, TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be positive.");

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than zero.");

        Name = name;
        Duration = duration;
        Price = price;
    }

}