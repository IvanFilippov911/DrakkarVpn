namespace DrakkarVpn.Core.Api.Modules.Users.Domain;

public sealed class AppUser
{
    public Guid Id { get; private set; }
    public long TelegramId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;

    private AppUser() { }

    private AppUser(Guid id, long telegramId, DateTime createdAt)
    {
        Id = id;
        TelegramId = telegramId;
        CreatedAt = createdAt;
        Status = UserStatus.Active;
    }

    public static AppUser CreateNew(long telegramId, DateTime nowUtc) =>
        new(Guid.NewGuid(), telegramId, nowUtc);
    
    public void SetStatus(UserStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(UserStatus), newStatus))
            throw new ArgumentOutOfRangeException(nameof(newStatus));
        if (Status == newStatus) return;
        Status = newStatus;
    }

    public void Ban()   => Status = UserStatus.Banned;
    public void Unban() => Status = UserStatus.Active;
}