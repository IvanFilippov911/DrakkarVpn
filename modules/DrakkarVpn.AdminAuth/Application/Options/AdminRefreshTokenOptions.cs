namespace DrakkarVpn.AdminAuth.Application.Options;

public sealed class AdminRefreshTokenOptions
{
    public const string SectionName = "AdminRefreshToken";

    public int LifetimeDays { get; set; } = 7;
}
