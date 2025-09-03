using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Users.Domain;

public sealed class AppUser
{
    public Guid Id { get; private set; }
    public TelegramId TelegramId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;

    private AppUser() { }

    private AppUser(Guid id, TelegramId telegramId, DateTime createdAt)
    {
        Id = id;
        TelegramId = telegramId;
        CreatedAt = createdAt;
        Status = UserStatus.Active;
    }

    public static AppUser CreateNew(TelegramId telegramId, DateTime nowUtc) =>
        new(Guid.NewGuid(), telegramId, nowUtc);

    public void Ban()   => Status = UserStatus.Banned;
    public void Unban() => Status = UserStatus.Active;
}