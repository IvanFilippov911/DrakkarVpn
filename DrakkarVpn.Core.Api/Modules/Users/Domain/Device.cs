namespace DrakkarVpn.Core.Api.Modules.Users.Domain;

public sealed class Device
{
    public string DeviceId { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public string? Name { get; private set; }
    public string? Platform { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastSeen { get; private set; }
    public DeviceStatus Status { get; private set; }

    private Device() { }

    public static Device Create(string deviceId, Guid userId, string? name, string? platform, DateTime nowUtc) =>
        new()
        {
            DeviceId = deviceId,
            UserId = userId,
            Name = name,
            Platform = platform,
            CreatedAt = nowUtc,
            Status = DeviceStatus.Active
        };

    public void Touch(string? name, string? platform, DateTime nowUtc)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(platform)) Platform = platform;
        LastSeen = nowUtc;
    }

    public void Revoke() => Status = DeviceStatus.Revoked;
}