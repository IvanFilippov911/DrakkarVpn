namespace DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;

public readonly record struct TariffId(Guid Value)
{
    public static TariffId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}