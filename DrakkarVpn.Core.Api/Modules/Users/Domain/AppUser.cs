namespace DrakkarVpn.Core.Api.Modules.Users.Domain;

public sealed class AppUser
{
    public Guid Id { get; private set; }
    public long TelegramId { get; private set; }
    public string? Username  { get; private set; }  
    public DateTime CreatedAt { get; private set; }
    public UserStatus Status { get; private set; }
    public bool IsInternal { get; private set; }
    public string? BanReason { get; private set; }
    public DateTime? BannedAtUtc { get; private set; }
    
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
    
    public void Ban(string reason, DateTime nowUtc)
    {
        if (Status == UserStatus.Banned)
            return;

        Status       = UserStatus.Banned;
        BanReason   = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        BannedAtUtc  = nowUtc;
    }

    public void Unban()
    {
        if (Status == UserStatus.Active)
            return;

        Status       = UserStatus.Active;
        BanReason    = null;
        BannedAtUtc  = null;
    }

    public void SetInternal(bool isInternal)
    {
        IsInternal = isInternal;
    }
    
    public void UpdateUsername(string? username)
    {
        Username = string.IsNullOrWhiteSpace(username)
            ? null
            : username.Trim();
    }
}