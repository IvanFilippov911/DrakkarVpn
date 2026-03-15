namespace DrakkarVpn.AdminAuth.Application.Options;

public sealed class AdminLockoutOptions
{
    public const string SectionName = "AdminLockout";

    public bool AllowedForNewUsers { get; set; } = true;
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public int DefaultLockoutMinutes { get; set; } = 15;
}
