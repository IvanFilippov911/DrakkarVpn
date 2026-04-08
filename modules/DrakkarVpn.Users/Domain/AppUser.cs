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
    public DateTime? ModerationUpdatedAtUtc { get; private set; }
    public DateTime? TrialGrantedAtUtc { get; private set; }
    
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
    
    
    public void SetInternal(bool isInternal, DateTime nowUtc)
    {
        if (IsInternal == isInternal)
            return;

        IsInternal = isInternal;
        ModerationUpdatedAtUtc = nowUtc;
    }

    public void MarkTrialGranted(DateTime grantedAtUtc)
    {
        if (TrialGrantedAtUtc is not null)
            return;

        TrialGrantedAtUtc = grantedAtUtc;
    }
}