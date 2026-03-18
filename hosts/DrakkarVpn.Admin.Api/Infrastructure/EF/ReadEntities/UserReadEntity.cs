namespace DrakkarVpn.Users.Infrastructure.EF.Entity;

public sealed class UserReadEntity
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }             
    public int Status { get; set; }                    
    public bool IsInternal { get; set; }
    public string? BanReason { get; set; }
    public DateTime? BannedAtUtc { get; set; }
    public DateTime? ModerationUpdatedAtUtc { get; set; }
}