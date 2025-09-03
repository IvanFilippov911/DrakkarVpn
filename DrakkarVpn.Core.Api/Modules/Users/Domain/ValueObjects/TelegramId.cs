namespace DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;

public readonly record struct TelegramId(long Value)
{
    public static TelegramId Create(long value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "TelegramId must be > 0");
        return new TelegramId(value);
    }

    public override string ToString() => Value.ToString();

    public static explicit operator long(TelegramId id) => id.Value;
    public static explicit operator TelegramId(long value) => Create(value);
}